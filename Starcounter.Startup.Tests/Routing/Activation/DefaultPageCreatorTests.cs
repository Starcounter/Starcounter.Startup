using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Activation;
using Starcounter.Startup.Tests.Routing.Middleware.ExamplePages;
using Starcounter.Startup.Tests.TestModel;

namespace Starcounter.Startup.Tests.Routing.Activation
{
    public class DefaultPageCreatorTests
    {
        private DefaultPageCreator _sut;
        private const string InjectedValue = "injected value";

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(InjectedValue)
                .BuildServiceProvider();
            _sut = new DefaultPageCreator(serviceProvider);
        }
        [Test]
        public void ShouldCallInit()
        {
            var page = CreatePage<InitPage>();

            page.HasBeenInitialized.Should().BeTrue();
        }

        [Test]
        public void ShouldHandleConstructorInjection()
        {
            var page = CreatePage<ConstructorInjection>();

            page.Injected.Should().Be(InjectedValue);
        }

        [Test]
        public void ShouldCallHandleContextSupport()
        {
            var context = new Thing();
            var page = CreatePage<ContextPage>(context);

            page.Context.Should().BeSameAs(context);
        }

        [Test]
        public void ShouldThrowMeaningfulExceptionWhenIInitWithDependenciesIsBadlyImplemented()
        {
            new object().Invoking(o => CreatePage<BadlyImplementedDependencies>())
                .Should().Throw<InvalidOperationException>()
                .WithInnerException<InvalidOperationException>()
                .WithMessage(StringsFormatted.DefaultPageCreator_TypeImplementsInitWithDependenciesBadly());
        }

        private T CreatePage<T>(object context = null)
        {
            var response = _sut.Create(new RoutingInfo()
            {
                SelectedPageType = typeof(T),
                Context = context
            });

            response.Resource.Should().BeOfType(typeof(T));
            return (T) response.Resource;
        }

#pragma warning disable 618
        private class BadlyImplementedDependencies : Json, IInitPageWithDependencies
#pragma warning restore 618
        {

        }

        private class ConstructorInjection: Json
        {
            public string Injected { get; }

            public ConstructorInjection(string injected)
            {
                Injected = injected;
            }
        }

#pragma warning disable 618
        private class InitPage : Json, IInitPage
#pragma warning restore 618
        {
            public bool HasBeenInitialized { get; private set; }

            public void Init()
            {
                HasBeenInitialized = true;
            }
        }

    }
}
