using Starcounter.Startup.Routing;
using Starcounter.Startup.Tests.TestModel;

namespace Starcounter.Startup.Tests.Routing.Middleware.ExamplePages
{
    public class BoundContextPage : Json, IBound<Thing>, IPageContext<Thing>
    {
        public Thing Context { get; set; }

        public void HandleContext(Thing context)
        {
            Context = context;
        }
    }
}