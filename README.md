# Starcounter.Startup
Dependency injection for Starcounter 2.4

## Installation

[This package is available on nuget](https://www.nuget.org/packages/Starcounter.Startup/). You can get it there. To install with CLI run:

```
Install-Package Starcounter.Startup
```

## Getting started

Create a startup class (commonly called `Startup`). It has to implement `Starcounter.Startup.Abstractions.IStartup`.

```c#
using Microsoft.Extensions.DependencyInjection;
using Starcounter.Startup.Abstractions;

public class Startup : IStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // here you can configure your DI container
    }

    public void Configure(IApplicationBuilder applicationBuilder)
    {
        // here you perform any start-up tasks for your application
        // applicationBuilder can be used to access the IServiceProvider
    }
}
```

`IStartup` mimics asp.net core's startup concept. You can read about it on [docs.microsoft.com](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1)
In your `Program.cs` you should have only a single call to the bootstrapper:

```c#
using Starcounter.Startup;

public class Program
{
    public static void Main()
    {
        DefaultStarcounterBootstrapper.Start(new Startup());
    }
}
```

This will start your application according to the `Startup` class.

## Router

`Router` is a class that helps creating the view-models and register them with URIs. To use it you have to add it to the DI container in `ConfigureServices`:
```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services.AddRouter();
}
```

### Registering view-models with UrlAttribute

You can annotate your view-model with `Starcounter.Startup.Routing.UrlAttribute`:
```c#
// using Starcounter.Startup.Routing;

[Url("/DogsApp/Dogs")]
[Url("/DogsApp/AllDogs")] // you can use UrlAttribute multiple times
public partial class DogViewModel: Json
{
    // ...
}
```
⚠️ Be careful to use `UrlAttribute` from `Starcounter.Startup.Routing` namespace. Common mistake is to use one from `System.Runtime.Remoting.Activation` instead.

You have to tell the `Router` to scan your assembly for all view-models to register them:
```c#
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void Configure(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.ApplicationServices.GetRouter().RegisterAllFromCurrentAssembly();
    // see also RegisterAllFromAssembly() if you keep your view-models in a separate project
}
```

The snippets above will register a handler under "/DogsApp/Dogs" and "/DogsApp/AllDogs". This handler will return `DogViewModel`.

### Registering view-models manually

Sometimes you want to have more control over the registration of your handlers. You can achieve that with manual registration:

```c#
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void Configure(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.ApplicationServices.GetRouter()
      .HandleGet<DogViewModel>("/DogsApp/partial/Dogs", new HandlerOptions {SelfOnly = true});
    // see also other overloads of HandleGet
}
```

The snippet above will register a handler under "/DogsApp/partial/Dogs". This handler will return `DogViewModel`, but will only be available via `Self.Get`.

## Dependency injection in view-models

To use services from the DI container in your view-model, declare a constructor that accepts dependencies as arguments. For more information about Dependency Injection, consult [microsoft docs on DI](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection). 

```c#
public partial class DogViewModel: Json
{
    public DogViewModel(IDogService dogService)
    {
      _dogService = dogService;
    }
}
```

For a long time, Starcounter didn't support constructor injection, and used `IInitPageWithDependncies` marker interface instead. You would it and create public, non-static, void `Init` method that accepted your dependencies as parameters. Below is an example of that practice. It can be now safely converted to constructor injection.

```c#
// using Starcounter.Startup.Routing.Activation;
// LEGACY CODE

public partial class DogViewModel: Json, IInitPageWithDependencies
{
    public void Init(IDogService dogService)
    {
      _dogService = dogService;
    }
}
```

## Services registered by default

`DefaultStarcounterBootstrapper` registers two aspnet.core's features by default - [logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1) and [options](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1&tabs=basicconfiguration). You can read about them on [docs.microsoft.com](https://docs.microsoft.com).
All logs are printed on Standard Output by default.

## Middleware

Sometimes you want to define a behavior that will be applied to all the requests processed by the `Router`.
To achieve this, implement `Starcounter.Startup.Routing.IPageMiddleware` interface and register it in the DI container.

```c#
using System;
using Microsoft.Extensions.Logging;
using Starcounter;
using Starcounter.Startup.Routing;

public class LoggingMiddleware : IPageMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public Response Run(RoutingInfo routingInfo, Func<Response> next)
    {
        _logger.LogInformation("Processing request");
        return next();
    }
}
```

```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services.AddRouter();
    services.AddTransient<IPageMiddleware, LoggingMiddleware>();
}
```

The above snippet will register `LoggingMiddleware` to run at every request processed by the `Router`.

`AddRouter` extension method mentioned before adds two pieces of middleware by default: `DbScopeMiddleware` and `ContextMiddleware`. If you want to prevent that behavior you can do that by passing `false` to `includeDefaultMiddleware` parameter:

```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services.AddRouter(false);
    // you can add them manually:
    // services.AddDbScopeMiddleware();
    // services.AddTransient<IPageMiddleware, ContextMiddleware>();
}
```

## Recipe: MasterPage

To wrap every page marked with a specific attribute in a common view-model, you can use the following code:
```c#
using System;
using System.Reflection;
using Starcounter;
using Starcounter.Startup.Routing;

[AttributeUsage(AttributeTargets.Class)]
public sealed class WrapAttribute : Attribute {}

public class WrapperViewModel : Json
{
    public Json InnerViewModel { get; set; }
}

public class WrapperMiddleware : IPageMiddleware
{
    public Response Run(RoutingInfo routingInfo, Func<Response> next)
    {
        var originalResponse = next();
        if (routingInfo.SelectedPageType.GetCustomAttribute<WrapAttribute>() == null)
        {
            return originalResponse;
        }

        return new WrapperViewModel {InnerViewModel = originalResponse};
    }
}
```

Remember to register it:
```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services.AddRouter();
    services.AddTransient<IPageMiddleware, WrapperMiddleware>();
}
```

To use it, mark your view-models with your new attribute:
```c#
using Starcounter;
using Starcounter.Startup.Routing;

[Url("/DogsApp/Dogs")]
[Wrap]
public partial class DogsViewModel: Json
{
  // ...
}
```
