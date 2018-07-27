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
        public void AddRouterConfiguresService()
        {
            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .AddLogging(builder => builder.ClearProviders());

            serviceCollection.AddRouter();

            serviceCollection.BuildServiceProvider().GetService<Router>()
                .Should().NotBeNull();
        }

        [Test]
        public void AddDbScopeMiddlewareConfiguresService()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbScopeMiddleware();

            serviceCollection.BuildServiceProvider().GetServices<IPageMiddleware>()
                .Should().ContainItemsAssignableTo<DbScopeMiddleware>();
        }
    }
}