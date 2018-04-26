using Starcounter.Startup.Routing;
using Starcounter.Startup.Tests.TestModel;

namespace Starcounter.Startup.Tests.Routing.Middleware.ExamplePages
{
    public class BoundContextPageWithDifferentTypes : Json, IBound<Thing>, IPageContext<ThingItem>
    {
        public ThingItem Context { get; private set; }

        public void HandleContext(ThingItem context)
        {
            Context = context;
        }
    }
}