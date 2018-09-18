using FluentAssertions;
using NUnit.Framework;
using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Tests.Routing.Middleware
{
    public class PartialUriTransformerTests
    {
        private PartialUriHelper _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new PartialUriHelper(new FakeApplicationNameProvider());
        }

        [TestCase("/fake/partial/dogs", "/fake/dogs")]
        [TestCase("/FAKE/PARTIAL/dogs", "/fake/dogs")]
        [TestCase("/fake/partial/dogs/1", "/fake/dogs/1")]
        [TestCase("/fake/partial/dogs/1/tail", "/fake/dogs/1/tail")]
        public void TestPartialToPage(string inputPartialUri, string expectedOutput)
        {
            _sut.PartialToPage(inputPartialUri).Should().Be(expectedOutput);
        }

        [TestCase("/fake/dogs", "/fake/partial/dogs")]
        [TestCase("/FAKE/dogs", "/fake/partial/dogs")]
        [TestCase("/fake/dogs/1", "/fake/partial/dogs/1")]
        [TestCase("/fake/dogs/1/tail", "/fake/partial/dogs/1/tail")]
        public void TestPageToPartial(string inputPageUri, string expectedOutput)
        {
            _sut.PageToPartial(inputPageUri).Should().Be(expectedOutput);
        }

        [TestCase("/fake/dogs", false)]
        [TestCase("/fake/partial/dogs/1", true)]
        [TestCase("/FAKE/PARTIAL/dogs/1", true)] // Self.GET lowers case of blended URIs
        [TestCase("/fake/partialdogs/2", false)]
        public void TestIsPartialUri(string inputUri, bool expectedOutput)
        {
            _sut.IsPartialUri(inputUri).Should().Be(expectedOutput);
        }
    }
}