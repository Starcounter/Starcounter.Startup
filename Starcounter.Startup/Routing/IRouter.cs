using System;

namespace Starcounter.Startup.Routing
{
    public interface IRouter
    {
        /// <summary>
        /// Registers specified view-model type according to its <see cref="UrlAttribute"/>
        /// </summary>
        /// <param name="viewModelType">The view-model type to register</param>
        /// <param name="handlerOptions">Optional handler options</param>
        void HandleGet(Type viewModelType, HandlerOptions handlerOptions = null);

        /// <summary>
        /// Registers specified view-model type ignoring its <see cref="UrlAttribute"/> and using
        /// provided URI
        /// </summary>
        /// <param name="uri">The URI to register the view-model</param>
        /// <param name="viewModelType">The view-model type to register</param>
        /// <param name="handlerOptions">Optional handler options</param>
        void HandleGet(string uri, Type viewModelType, HandlerOptions handlerOptions = null);
    }
}