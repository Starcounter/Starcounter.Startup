﻿using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Tests.Routing
{
    public class RouterServiceCollectionTests
    {
        [Test]
        public void AddRouterConfiguresRouter()
        {
            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .AddLogging(builder => builder.ClearProviders());

            serviceCollection.AddRouter();

            serviceCollection.BuildServiceProvider().GetService<IRouter>()
                .Should().NotBeNull();
        }

        [Test]
        public void AddRouterConfiguresDefaultMiddlewareIfAsked()
        {
            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .AddLogging(builder => builder.ClearProviders());

            serviceCollection.AddRouter(true);

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Select(middleware => middleware.GetType())
                .Should().ContainInOrder(typeof(MasterPageMiddleware), typeof(ContextMiddleware));
        }

        [Test]
        public void AddRouterDoesntConfiguresMiddlewareIfNotAsked()
        {
            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .AddLogging(builder => builder.ClearProviders());

            serviceCollection.AddRouter(false);

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Should().BeEmpty();
        }

        [Test]
        public void AddInteractionScopeMiddlewareConfiguresService()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddInteractionScopeMiddleware();

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Should().ContainItemsAssignableTo<InteractionScopeMiddleware>();
        }

        [Test]
        public void AddInteractionScopeMiddlewareCalledIsIdempotent()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddInteractionScopeMiddleware()
                .AddInteractionScopeMiddleware();

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Should().HaveCount(1);
        }

    }
}