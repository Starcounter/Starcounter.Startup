using Starcounter.Startup.Routing.Activation;

namespace Starcounter.Startup.Tests.Routing.Middleware.ExamplePages
{
    public class InitPage : Json, IInitPage
    {
        public bool HasBeenInitialized { get; set; }

        public void Init()
        {
            HasBeenInitialized = true;
        }
    }
}