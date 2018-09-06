using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starcounter.Startup.Routing.Activation;
using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Routing
{
    public static class RouterServiceCollectionExtensions
    {
        public static IServiceCollection AddRouter(this IServiceCollection services, bool addDefaultMiddleware = true)
        {
            services.TryAddSingleton<IPageCreator, DefaultPageCreator>();
            services.TryAddTransient<Router>();

            if (addDefaultMiddleware)
            {
                // order of those is important. That way, ContextMiddleware will run inside DbScope
                services.AddDbScopeMiddleware();
                services.AddContextMiddleware();
            }

            return services;
        }

        public static Router GetRouter(this IServiceProvider services) =>
            services.GetRequiredService<Router>();

        public static IServiceCollection AddDbScopeMiddleware(this IServiceCollection services, bool enableScopeByDefault = true)
        {
            services
                .TryAddEnumerable(
                    ServiceDescriptor.Transient<IPageMiddleware, DbScopeMiddleware>((_) => new DbScopeMiddleware(enableScopeByDefault)));
            return services;
        }

        public static IServiceCollection AddContextMiddleware(this IServiceCollection services)
        {
            services.TryAddTransient<IObjectRetriever, DatabaseObjectRetriever>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IPageMiddleware, ContextMiddleware>());
            return services;
        }
    }
}