using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            serviceCollection.Replace(
                ServiceDescriptor.Transient<IApplicationNameProvider, FakeApplicationNameProvider>());

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
                .Should().ContainInOrder(typeof(DbScopeMiddleware), typeof(ContextMiddleware));
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
        public void AddDbScopeMiddlewareConfiguresService()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbScopeMiddleware();

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Should().ContainItemsAssignableTo<DbScopeMiddleware>();
        }

        [Test]
        public void AddDbScopeMiddlewareCalledIsIdempotent()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddDbScopeMiddleware()
                .AddDbScopeMiddleware();

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Should().HaveCount(1);
        }

    }
}