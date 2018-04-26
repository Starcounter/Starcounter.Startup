﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starcounter.Startup.Routing.Activation;

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

    }
}