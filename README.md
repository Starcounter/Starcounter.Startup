# Starcounter.Startup
Dependency injection for Starcounter 2.4

## Table of contents

- [Installation](#installation)
- [Getting started](#getting-started)
- [Router](#router)
  * [Registering view-models with UrlAttribute](#registering-view-models-with-urlattribute)
  * [Exposing view-models to blending and browser](#exposing-view-models-to-blending-and-browser)
  * [Registering view-models manually](#registering-view-models-manually)
  * [Handling URI parameters, working with Context](#handling-uri-parameters-working-with-context)
  * [Middleware](#middleware)
  * [Blending, Db.Scope, MasterPage](#blending-dbscope-masterpage)
    + [Controlling transaction scopes in master page](#controlling-transaction-scopes-in-master-page)
- [Dependency injection in view-models](#dependency-injection-in-view-models)
  * [Services registered by default](#services-registered-by-default)
- [UriHelper](#urihelper)

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

### Exposing view-models to blending and browser

By default, applying `[UrlAttribute]` will expose your view-model to the browser (or anyone who uses HTTP) under the supplied URI, and to the Blending Engine under the partial URI.

```c#
// using Starcounter.Startup.Routing;

[Url("/DogsApp/Dogs/{?}")]
public partial class DogViewModel: Json
{
    // ...
}
```

The code above will expose your view-model under `/DogsApp/Dogs/{?}` to the browser, and `/DogsApp/partial/Dogs/{?}` to the Blending Engine. You won't be able to call the second URI from the browser.

You can also expose your view-model to Blending or browser only.

```c#
// blending only
[Url("/DogsApp/Dogs/{?}", External = false)]

// browser only
[Url("/DogsApp/Dogs/{?}", Blendable = false)]
```

### Registering view-models manually

Sometimes you want to have more control over the registration of your handlers. You can achieve that with manual registration:

```c#
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void Configure(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.ApplicationServices.GetRouter()
      .HandleGet<DogViewModel>("/DogsApp/very-custom-uri/Dogs", new HandlerOptions {SelfOnly = true});
    // see also other overloads of HandleGet
}
```

The snippet above will register a handler under "/DogsApp/very-custom-uri/Dogs". This handler will return `DogViewModel`, but will only be available to Blending engine.

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
    // the name of this method is irrelevant, but calling it UriToContext is a good practice
    public static Dog UriToContext(string[] args, IDogsRepository dogsRepository)
    {
        // args is guaranteed to have one element, because its only URI has only one parameter
        // returning null will cause the Router to respond with 404
        return dogsRepository.GetByName(args[0]);
    }

    // ...
}
```

In this example instead of fetching the Context by its ID, we fetch it using its `Name` property.
To use `[UriToContext]`, apply it to one method that:

* is `public static`
* has return type assignable to Context type
* has the first parameter of type `string[]`

This method will be invoked before the view-model is created. It will be passed URI parameters as its sole argument. If it returns null, the `Router` will respond with `404`. Otherwise, the return value will be used as the `Context`.
This method can accept more than one parameter. Any additional parameters will be treated as a dependency and resolved using Dependency Injection container.

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

`AddRouter` extension method mentioned before adds two pieces of middleware by default: `MasterPageMiddleware` and `ContextMiddleware`. If you want to prevent that behavior you can do that by passing `false` to `includeDefaultMiddleware` parameter:

```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services.AddRouter(false);
    // you can add them manually:
    // services.AddTransient<IPageMiddleware, MasterPageMiddleware>();
    // services.AddTransient<IPageMiddleware, ContextMiddleware>();
}
```

### Blending, Db.Scope, MasterPage

By default, every view-model you register with `[Url]` is both page URI and partial URI registered. When you access its page URI, `Router` creates a `Db.Scope` and retrieves its blending URI. This means, that if you request your view-model in the browser, the response can contain other, blended view-models as well. `Router` makes sure that they all share a common transaction.

A common application feature is to have some layout that wraps every response of an app and adds navigation features. This wrapping page would be called a master page. To enable it, create a view-model deriving from `MasterPageBase` and register it using `SetMasterPage<T>`:

```json
{
  "Html": "/MyApplication/views/MasterNavigation.html",
  "InnerJson": {}
}
```

```c#
using Starcounter;
using Starcounter.Startup.Routing.Middleware;

public partial class MasterNavigationPage : MasterPageBase
{
    public override void SetPartial(Json partial)
    {
        InnerJson = partial;
    }
}
```

```html
<template>
    <dom-bind>
        <template is="dom-bind">
            <h1>Application-wide header</h1>
            <a href="/MyApplication/Home">Go home</a>
            <starcounter-include view-model="{{model.InnerJson}}"></starcounter-include>
        </template>
    </dom-bind>
</template>
```

```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services
        .AddRouter()
        .SetMasterPage<MasterNavigationPage>();
    
}
```


#### Controlling transaction scopes in master page

Without master page, all the blended view-models share a common transaction. With a master page like one defined above, all the blended view-models share a common transaction, but the master page itself has no transaction. You can change that if you want.

To put the master page in a transaction, you have to create it in `Db.Scope`. To do it, register your custom master page factory:

```c#
// using Microsoft.Extensions.DependencyInjection;
// using Starcounter.Startup.Abstractions;
// using Starcounter.Startup.Routing;

public void ConfigureServices(IServiceCollection services)
{
    services
        .AddRouter()
        .SetMasterPage((provider, routingInfo) => Db.Scope(() => new MasterNavigationPage()))
}
```

The code above will create `MasterNavigationPage` in a transaction scope, but it will not be shared with the blended view-models. If you want it to be in shared transaction, you can change it by overriding `ExecuteInScope` in your master page:

```c#
using Starcounter;
using Starcounter.Startup.Routing.Middleware;

public partial class MasterNavigationPage : MasterPageBase
{
    public override void SetPartial(Json partial)
    {
        InnerJson = partial;
    }

    public override T ExecuteInScope(Func<T> innerJsonFactory)
    {
        return AttachedScope.Scope(innerJsonFactory);
    }
}
```

The code above will attach the blended view-models to the scope of the master page. That way all the view-models will share a transaction.

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

For a long time, Starcounter didn't support constructor injection, and used `IInitPageWithDependncies` marker interface instead. You would implement it and create public, non-static, void `Init` method that accepted your dependencies as parameters. Below is an example of that practice. It can be now safely converted to constructor injection.

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

⚠️Only view-models created by the Router (those i.e. created automatically by accessing a URI) will have their dependncies filled. View-models nested inside other view-model, that are created by Starcounter, will not automatically be created with dependencies.

```c#
// WON'T WORK

// AllDogsViewModel.json
{
    "Children": [ {} ]
}

// AllDogsViewModel.json.cs
public partial class AllDogsViewModel: Json
{
    public DogViewModel(IDogService dogService)
    {
        Children.Data = dogService.GetAllDogs();
    }

    [AllDogsViewModel_json.Children]
    public partial class ChildViewModel: Json
    {
        // this won't even compile
        public ChildViewModel(IDogService dogService)
        {
            // ...
        }
    }
}
```

To fill dependencies for a nested view-model you have to create it by hand:

```c#
// AllDogsViewModel.json.cs
public partial class AllDogsViewModel: Json
{
    public DogViewModel(IDogService dogService)
    {
        foreach (var dog in dogService.GetAllDogs())
        {
            Children.Add().Init(dogService);
        }
    }

    [AllDogsViewModel_json.Children]
    public partial class ChildViewModel: Json
    {
        public void Init(IDogService dogService)
        {
            // ...
        }
    }
}
```

### Services registered by default

`DefaultStarcounterBootstrapper` registers two aspnet.core's features by default - [logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1) and [options](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1&tabs=basicconfiguration). You can read about them on [docs.microsoft.com](https://docs.microsoft.com).
All logs are printed on Standard Output by default.

## UriHelper

`UriHelper` is a collection of static methods which ease working with Starcounter URIs. It exposes following methods. Each method below is accompanied by an example with output.

```c#
public static string PartialToPage(string partialUri)
```

Converts partial URI to page URI. E.g. `PartialToPage("/MyApp/partial/dog")` will return `"/MyApp/dog"`.


```c#
public static string PageToPartial(string pageUri)
```

Converts page URI to partial URI. E.g. `PageToPartial("/MyApp/dog")` will return `"/MyApp/partial/dog"`.

```c#
public static bool IsPartialUri(string uri)
```

Returns true if the supplied URI is a partial URI. E.g. `IsPartialUri("/MyApp/partial/dog")` will return `true`, but `IsPartialUri("/MyApp/dog")` will return `false`.

```c#
public static string WithArguments(string uriTemplate, params string[] arguments)
```

Returns the supplied URI with its arguments filled. E.g. `WithArguments("/MyApp/partial/dog/{?}", "xyz")` will return `"/MyApp/partial/dog/xyz"`.
