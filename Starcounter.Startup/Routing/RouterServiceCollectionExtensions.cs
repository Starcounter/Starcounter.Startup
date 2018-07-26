using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starcounter.Startup.Routing.Activation;
using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Routing
{
    public static class RouterServiceCollectionExtensions
    {
        public static IServiceCollection AddRouter(this IServiceCollection services)
        {
            services.TryAddSingleton<IPageCreator, DefaultPageCreator>();
            services.TryAddTransient<Router>();

            return services;
        }

        public static Router GetRouter(this IServiceProvider services) =>
            services.GetRequiredService<Router>();

        public static IServiceCollection AddDbScopeMiddleware(this IServiceCollection services, bool enableScopeByDefault = true)
        {
            services
                .TryAddEnumerable(
                    ServiceDescriptor.Transient<IPageMiddleware>((_) => new DbScopeMiddleware(enableScopeByDefault)));
            return services;
        }

    }
}