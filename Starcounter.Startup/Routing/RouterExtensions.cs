using System;
using System.Linq;
using System.Reflection;

namespace Starcounter.Startup.Routing
{
    public static class RouterExtensions
    {
        /// <summary>
        /// Registers all view-model types found in specified assembly.
        /// </summary>
        /// <param name="router">The router to register in.</param>
        /// <param name="assembly">The assembly containing view-models to register.</param>
        public static void RegisterAllFromAssembly(this IRouter router, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes()
                .Where(type => type.GetCustomAttributes<UrlAttribute>().Any()))
            {
                router.HandleGet(type);
            }
        }

        /// <summary>
        /// Registers all view-model types found in the current assembly.
        /// </summary>
        /// <param name="router">The router to register in.</param>
        public static void RegisterAllFromCurrentAssembly(this IRouter router)
        {
            RegisterAllFromAssembly(router, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Registers specified view-model type according to its <see cref="UrlAttribute"/>
        /// </summary>
        /// <typeparam name="T">The view-model type to register</typeparam>
        /// <param name="router">The router to register in.</param>
        /// <param name="handlerOptions">Optional handler options</param>
        public static void HandleGet<T>(this IRouter router, HandlerOptions handlerOptions = null)
        {
            router.HandleGet(typeof(T), handlerOptions);
        }

        /// <summary>
        /// Registers specified view-model type ignoring its <see cref="UrlAttribute"/> and using
        /// provided URI
        /// </summary>
        /// <param name="router">The router to register in.</param>
        /// <param name="url">The URI to register the view-model</param>
        /// <typeparam name="T">The view-model type to register</typeparam>
        /// <param name="handlerOptions">Optional handler options</param>
        public static void HandleGet<T>(this IRouter router, string url, HandlerOptions handlerOptions = null)
        {
            router.HandleGet(url, typeof(T), handlerOptions);
        }
    }
}