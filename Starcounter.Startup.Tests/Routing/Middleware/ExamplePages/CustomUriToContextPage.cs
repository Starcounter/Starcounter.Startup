using Starcounter.Startup.Routing;

namespace Starcounter.Startup.Tests.Routing.Middleware.ExamplePages
{
    public class CustomUriToContextPage : IPageContext<string[]>
    {
        [UriToContext]
        public static string[] UriToContext(string[] args)
        {
            return args;
        }

        public void HandleContext(string[] context)
        {
        }
    }
}