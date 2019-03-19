using System;
using System.Reflection;

namespace Starcounter.Startup.Routing.Middleware
{
    public class InteractionScopeMiddleware : IPageMiddleware
    {
        private readonly InteractionScopeAttribute.InteractionScopeMode _defaultValue;

        public InteractionScopeMiddleware(InteractionScopeAttribute.InteractionScopeMode defaultValue = InteractionScopeAttribute.InteractionScopeMode.AttachOrCreate)
        {
            _defaultValue = defaultValue;
        }

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            var attribute = routingInfo.SelectedPageType.GetCustomAttribute<InteractionScopeAttribute>();

            return GetResponseByMode(attribute?.Value ?? _defaultValue, next);
        }

        private Response GetResponseByMode(InteractionScopeAttribute.InteractionScopeMode mode, Func<Response> next)
        {
            switch (mode)
            {
                case InteractionScopeAttribute.InteractionScopeMode.AttachOrCreate:
                    return Db.Scope(next);
                case InteractionScopeAttribute.InteractionScopeMode.AlwaysCreate:
                    return new Transaction().Scope(next);
                case InteractionScopeAttribute.InteractionScopeMode.None:
                    return next();
            }

            return null;
        }
    }
}