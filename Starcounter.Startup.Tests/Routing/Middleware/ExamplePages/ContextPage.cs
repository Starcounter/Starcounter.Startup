using Starcounter.Startup.Routing;
using Starcounter.Startup.Tests.TestModel;

namespace Starcounter.Startup.Tests.Routing.Middleware.ExamplePages
{
    public class ContextPage : Json, IPageContext<Thing>
    {
        public Thing Context { get; set; }

        public void HandleContext(Thing context)
        {
            Context = context;
        }
    }
}