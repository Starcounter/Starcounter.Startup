using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Middleware;
using Starcounter.Startup.Tests.Routing.Middleware.ExamplePages;
using Starcounter.Startup.Tests.TestModel;

namespace Starcounter.Startup.Tests.Routing.Middleware
{
    public class ContextMiddlewareTests
    {
        private ContextMiddleware _contextMiddleware;
        private Mock<IObjectRetriever> _objectRetriever;
        private Mock<IServiceProvider> _serviceProviderMock;

        [SetUp]
        public void Setup()
        {
            _objectRetriever = new Mock<IObjectRetriever>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _contextMiddleware = new ContextMiddleware(_objectRetriever.Object, _serviceProviderMock.Object);
        }

        [Test]
        public void ShouldUseExplicitUriToContextWhenAvailable()
        {
            var arguments = new[] {"a", "b"};
            var routingInfo = new RoutingInfo
            {
                Arguments = arguments,
                SelectedPageType = typeof(CustomUriToContextPage)
            };

            _contextMiddleware.Run(routingInfo, () => new Response());

            routingInfo.Context.Should().Be(arguments);
        }

        [Test]
        public void ShouldInjectServicesIntoExplicitUriToContext()
        {
            var injectedStrings = new[] {"a", "b"};
            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IEnumerable<string>)))
                .Returns(injectedStrings);
            var routingInfo = new RoutingInfo
            {
                Arguments = new[] {""},
                SelectedPageType = typeof(CustomUriToContextPageWithDependencies)
            };

            _contextMiddleware.Run(routingInfo, () => new Response());

            routingInfo.Context.Should().BeOfType<string[]>().Which.Should().Equal(injectedStrings);
        }

        [Test]
        public void ShouldThrowWhenUriToContextDiFails()
        {
            // ARRANGE
            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IEnumerable<string>)))
                .Returns(null);
            var viewModelType = typeof(CustomUriToContextPageWithDependencies);
            var routingInfo = new RoutingInfo
            {
                Arguments = new[] {""},
                SelectedPageType = viewModelType
            };

            var uriToContext = viewModelType
                .GetMethod(nameof(CustomUriToContextPageWithDependencies.UriToContext));
            var uriToContextDependency = uriToContext.GetParameters().ToList()[1];

            // ACT
            _contextMiddleware.Invoking(cm => cm.Run(routingInfo, () => new Response()))
            // ASSERT
                .Should().Throw<InvalidOperationException>()
                .WithMessage(StringsFormatted.ContextMiddleware_CouldNotCreateContext(typeof(string[]),
                    viewModelType,
                    StringsFormatted.ContextMiddleware_CouldNotResolveUriToContextDependencies(uriToContext)))
                .WithInnerException<InvalidOperationException>()
                .WithMessage(StringsFormatted.ReflectionHelper_CouldNotInstantiateParameter(uriToContextDependency));
        }

        [Test]
        public void ShouldUseDbWhenPageContextIsExplicitButNoUriToContext()
        {
            var arguments = new[] {"a"};
            var routingInfo = new RoutingInfo
            {
                Arguments = arguments,
                SelectedPageType = typeof(ContextPage)
            };
            var thing = new Thing();
            _objectRetriever
                .Setup(retriever => retriever.GetById(typeof(Thing), "a"))
                .Returns(thing);

            _contextMiddleware.Run(routingInfo, () => new Response());

            routingInfo.Context.Should().Be(thing);
        }

        [Test]
        public void ShouldUseDbWhenPageIsIBoundButNoUriToContext()
        {
            var arguments = new[] {"a"};
            var routingInfo = new RoutingInfo
            {
                Arguments = arguments,
                SelectedPageType = typeof(BoundPage)
            };
            var thing = new Thing();
            _objectRetriever
                .Setup(retriever => retriever.GetById(typeof(Thing), "a"))
                .Returns(thing);

            _contextMiddleware.Run(routingInfo, () => new Response());

            routingInfo.Context.Should().Be(thing);
        }

        private class CustomUriToContextPageWithDependencies : IPageContext<string[]>
        {
            [UriToContext]
            public static string[] UriToContext(string[] args, IEnumerable<string> dependency)
            {
                return dependency.ToArray();
            }

            public void HandleContext(string[] context)
            {
            }
        }

    }
}