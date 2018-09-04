using System;

namespace Starcounter.Startup.Routing
{
    public interface IPageMiddleware
    {
        Response Run(RoutingInfo routingInfo, Func<Response> next);
    }
}