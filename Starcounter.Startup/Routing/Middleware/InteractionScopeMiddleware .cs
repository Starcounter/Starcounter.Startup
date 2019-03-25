using System;
using System.Reflection;

namespace Starcounter.Startup.Routing.Middleware
{
    public class InteractionScopeMiddleware : IPageMiddleware
    {
        private readonly InteractionScopeMode _defaultValue;

        public InteractionScopeMiddleware(InteractionScopeMode defaultValue = InteractionScopeMode.AttachOrCreate)
        {
            _defaultValue = defaultValue;
        }

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            var attribute = routingInfo.SelectedPageType.GetCustomAttribute<InteractionScopeAttribute>();

            return GetResponseByMode(attribute?.Value ?? _defaultValue, next);
        }

        private Response GetResponseByMode(InteractionScopeMode mode, Func<Response> next)
        {
            switch (mode)
            {
                case InteractionScopeMode.AttachOrCreate:
                    return Db.Scope(next);
                case InteractionScopeMode.AlwaysCreate:
                    return new Transaction().Scope(next);
            }

            return null;
        }
    }
}