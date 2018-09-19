using System;
using FluentAssertions;
using NUnit.Framework;
using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Tests.Routing.Middleware
{
    public class UriHelperTests
    {
        [TestCase("/app/partial/dogs", "/app/dogs")]
        [TestCase("/APP/PARTIAL/dogs", "/app/dogs")]
        [TestCase("/app/partial/dogs/1", "/app/dogs/1")]
        [TestCase("/app/partial/dogs/1/tail", "/app/dogs/1/tail")]
        public void TestPartialToPage(string inputPartialUri, string expectedOutput)
        {
            UriHelper.PartialToPage(inputPartialUri)
                // BeEquivalentTo is case-insensitive
                .Should().BeEquivalentTo(expectedOutput);
        }

        [TestCase("/app/dogs", "/app/partial/dogs")]
        [TestCase("/APP/dogs", "/app/partial/dogs")]
        [TestCase("/app/dogs/1", "/app/partial/dogs/1")]
        [TestCase("/app/dogs/1/tail", "/app/partial/dogs/1/tail")]
        public void TestPageToPartial(string inputPageUri, string expectedOutput)
        {
            UriHelper.PageToPartial(inputPageUri)
                // BeEquivalentTo is case-insensitive
                .Should().BeEquivalentTo(expectedOutput);
        }

        [TestCase("/app/dogs", false)]
        [TestCase("/app/partial/dogs/1", true)]
        [TestCase("/APP/PARTIAL/dogs/1", true)] // Self.GET lowers case of blended URIs
        [TestCase("/app/partialdogs/2", false)]
        public void TestIsPartialUri(string inputUri, bool expectedOutput)
        {
            UriHelper.IsPartialUri(inputUri)
                .Should().Be(expectedOutput);
        }

        [TestCase("/app/dogs", new string[0], "/app/dogs")]
        [TestCase("/app/dogs/{?}", new []{"A"}, "/app/dogs/A")]
        [TestCase("/app/dogs/{?}/details", new []{"A"}, "/app/dogs/A/details")]
        [TestCase("/app/dogs/{?}/details/{?}", new []{"A", "B"}, "/app/dogs/A/details/B")]
        [TestCase("/app/dogs?{?}", new []{"p1=value,p2=value"}, "/app/dogs?p1=value,p2=value")]
        public void TestWithArguments(string inputUriTemplate, string[] arguments, string expectedUri)
        {
            UriHelper.WithArguments(inputUriTemplate, arguments)
                .Should().Be(expectedUri);
        }

        [Test]
        public void WithArgumentsThrowsWhenThereAreTooManySlots()
        {
            var uriTemplate = "/{?}/{?}";
            var arguments = new []{"one"};
            new Action(() => UriHelper.WithArguments(uriTemplate, arguments))
                .Should().Throw<ArgumentException>()
                .WithMessage(StringsFormatted.UriHelper_CantFillUriTemplateSlotCountMismatch(uriTemplate, arguments, 2));
        }

        [TestCase("/{?}/{?}", 2)]
        [TestCase("/noslots", 0)]
        public void WithArgumentsThrowsWhenThereAreNotEnoughSlots(string uriTemplate, int slotCount)
        {
            var arguments = new [] { "one", "two", "three" };
            new Action(() => UriHelper.WithArguments(uriTemplate, arguments))
                .Should().Throw<ArgumentException>()
                .WithMessage(StringsFormatted.UriHelper_CantFillUriTemplateSlotCountMismatch(uriTemplate, arguments, slotCount));
                ;
        }
    }
}