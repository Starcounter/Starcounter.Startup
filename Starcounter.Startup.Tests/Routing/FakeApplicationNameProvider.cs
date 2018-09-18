using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Tests.Routing
{
    public class FakeApplicationNameProvider : IApplicationNameProvider
    {
        public string CurrentApplicationName => "fake";
    }
}