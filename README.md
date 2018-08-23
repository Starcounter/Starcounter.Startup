# Starcounter.Startup
Dependency injection for Starcounter 2.4

## Table of contents

- [Installation](#installation)
- [Getting started](#getting-started)
- [Router](#router)
  * [Registering view-models with UrlAttribute](#registering-view-models-with-urlattribute)
  * [Registering view-models manually](#registering-view-models-manually)
  * [Handling URI parameters, working with Context](#handling-uri-parameters-working-with-context)
  * [Middleware](#middleware)
  * [Recipe: MasterPage](#recipe-masterpage)
- [Dependency injection in view-models](#dependency-injection-in-view-models)
  * [Services registered by default](#services-registered-by-default)

<small><i><a href='http://ecotrust-canada.github.io/markdown-toc/'>Table of contents generated with markdown-toc</a></i></small>


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


### Handling URI parameters, working with Context

In most cases when you use URI parameters, they correspond to a database entity of type `T` and your view-model implements `IBound<T>`. This case, illustrated below, is handled automatically:

```c#
// using Starcounter.Startup.Routing;

[Url("/DogsApp/Dog/{?}")]
public partial class DogViewModel: Json, IBound<Dog>
{
    // ...
}
```

In the example below, if you open URI `/DogsApp/Dog/123`, then:
* Router (more specifically, `ContextMiddleware`) will look for a `Dog` with id `123` in the database
* If it's not found, then `404` response will be returned
* If it's found, then `DogViewModel` will be initialized, and then its `Data` property will be populated with the `Dog` found in the database

This is usually the desired behavior and if it satisfies you, can skip the rest of this chapter.

To understand how this works you first need to know what Context is. Context is the data represented by URI arguments. If the URI has a `Dog` id a parameter `123` (`/DogsApp/Dog/123`), then Context is the `Dog` entity with id `123`.

If the view-model implements `IBound<T>`, then the type of the Context is inferred to be `T`. You can, however override this by implementing `IPageContext<T>` interface. Consider following example:

You want the following view-model to be accessible by ID of the `Dog`. i.e. accessing `/DogsApp/Dog/123/Owner` should initialize `DogOwnerViewModel.Data` to the *owner* of the `Dog` with id `123`.

```c#
// using Starcounter.Startup.Routing;

[Url("/DogsApp/Dog/{?}/Owner")]
public partial class DogOwnerViewModel: Json, IBound<Person>, IPageContext<Dog>
{
    public void HandleContext(Dog context)
    {
        this.Data = context.Owner;
    }

    // ...
}
```

Here, the context type would've been inferred to `Person`, but we explicitly declared it to be `Dog`. To implement the interface we also had to specify what happens with the context. If you don't implement this interface, but implement `IBound<T>`, the context is simply assigned to `Data` property.

But how is this context object fetched? By default, if the URI has only one parameter and Context is a database entity, the parameter value is used to fetch the Context from the database. However, if one of those conditions are not met or you want to override the default behavior, you must use `[UriToContext]`:

```c#
// using Starcounter.Startup.Routing;
// using Starcounter.Linq;

[Url("/DogsApp/DogByName/{?}")]
public partial class DogViewModel: Json, IBound<Dog>
{
    [UriToContext]
    public static Dog HandleContext(string[] args)
    {
        // args is guaranteed to have one element, because its only URI has only one parameter
        // returning null will cause the Router to respond with 404
        return DbLinq.Objects<Dog>().FirstOrDefault(dog => dog.Name == args[0]);
    }

    // ...
}
```

In this example instead of fetching the Context by its ID, we fetch it using its `Name` property.
To use `[UriToContext]`, apply it to one method that:

* is `public static`
* has return type assignable to Context type
* has only one parameter of type `string[]`

This method will be invoked before the view-model is created. It will be passed URI parameters as its sole argument. If it returns null, the `Router` will respond with `404`. Otherwise, the return value will be used as the `Context`.

`[UriToContext]` and `IPageMiddleware<T>` features are connected, but independent. You can use them both or just one them.

### Middleware

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

### Recipe: MasterPage

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

## Dependency injection in view-models

To use services from the DI container in your view-model, implement marker interface `IInitPageWithDependencies` and create public, non-static, void `Init` method that accepts your dependencies as parameters.
This method mimics constructor, because Starcounter 2.4 doesn't support custom constructors in Typed Json view-models.

```c#
// using Starcounter.Startup.Routing.Activation;

public partial class DogViewModel: Json, IInitPageWithDependencies
{
    public void Init(IDogService dogService)
    {
      _dogService = dogService;
    }
}
```

When an instance of this class is created by the `Router`, it will have its dependencies injected using `Init` method.

### Services registered by default

`DefaultStarcounterBootstrapper` registers two aspnet.core's features by default - [logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1) and [options](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1&tabs=basicconfiguration). You can read about them on [docs.microsoft.com](https://docs.microsoft.com).
All logs are printed on Standard Output by default.