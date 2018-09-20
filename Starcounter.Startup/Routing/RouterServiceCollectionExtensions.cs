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
            services.TryAddTransient<IRouter, Router>();

            if (addDefaultMiddleware)
            {
                // order of those is important. That way, ContextMiddleware will run inside DbScope
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPageMiddleware, MasterPageMiddleware>());
                services.AddContextMiddleware();
            }

            return services;
        }

        public static IRouter GetRouter(this IServiceProvider services) =>
            services.GetRequiredService<IRouter>();

        [Obsolete("MasterPageMiddleware is now responsible for creating a scope, and it's included by default in AddRouter")]
        public static IServiceCollection AddDbScopeMiddleware(this IServiceCollection services, bool enableScopeByDefault = true)
        {
            services
                .TryAddEnumerable(
                    ServiceDescriptor.Transient<IPageMiddleware, DbScopeMiddleware>((_) => new DbScopeMiddleware(enableScopeByDefault)));
            return services;
        }

        /// <summary>
        /// Sets the view-model type for the master page. All the responses will be wrapped in this type.
        /// </summary>
        /// <typeparam name="T">The view-model type.</typeparam>
        /// <param name="services">The service collection to register in.</param>
        /// <returns>The original service collection.</returns>
        public static IServiceCollection SetMasterPage<T>(this IServiceCollection services)
            where T : MasterPageBase
        {
            services.AddSingleton<Func<RoutingInfo, MasterPageBase>>(provider => (_) => ActivatorUtilities.CreateInstance<T>(provider));
            return services;
        }

        /// <summary>
        /// Sets the factory for the master page. All the responses will be wrapped in the master page.
        /// Use this method to control creation of the master page, e.g. wrap it in <see cref="Db.Scope"/>.
        /// </summary>
        /// <param name="services">The service collection to register in.</param>
        /// <param name="masterPageFactory">The factory for master page.</param>
        /// <returns>The original service collection.</returns>
        public static IServiceCollection SetMasterPage(this IServiceCollection services, Func<IServiceProvider, RoutingInfo, MasterPageBase> masterPageFactory)
        {
            services.AddSingleton<Func<RoutingInfo, MasterPageBase>>(provider => routingInfo => masterPageFactory(provider, routingInfo));
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