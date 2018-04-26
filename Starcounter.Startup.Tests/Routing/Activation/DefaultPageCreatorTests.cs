﻿using FluentAssertions;
using NUnit.Framework;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Activation;
using Starcounter.Startup.Tests.Routing.Middleware.ExamplePages;
using Starcounter.Startup.Tests.TestModel;

namespace Starcounter.Startup.Tests.Routing.Activation
{
    public class DefaultPageCreatorTests
    {
        [Test]
        public void ShouldCallInit()
        {
            var defaultPageCreator = new DefaultPageCreator();
            var response = defaultPageCreator.Create(new RoutingInfo() {SelectedPageType = typeof(InitPage)});

            response.Resource.As<InitPage>().HasBeenInitialized.Should().BeTrue();
        }

        [Test]
        public void ShouldCallHandleContextSupport()
        {
            var defaultPageCreator = new DefaultPageCreator();
            var context = new Thing();
            var response = defaultPageCreator.Create(
                    new RoutingInfo()
                    {
                        SelectedPageType = typeof(ContextPage),
                        Context = context
                    });

            response.Resource.As<ContextPage>().Context.Should().BeSameAs(context);
        }
    }
}