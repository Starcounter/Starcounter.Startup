using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Starcounter.Advanced;

namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Responsible for obtaining the page context using information from URI.
    /// </summary>
    /// This middleware will set the <see cref="RoutingInfo.Context"/> property in current <see cref="RoutingInfo"/>
    /// for a page that implements either <see cref="IPageContext{T}"/> or <see cref="Starcounter.IBound{DataType}"/>. If the selected page type
    /// has a method marked with <see cref="UriToContextAttribute"/>, then it will be used. Otherwise, this middleware will
    /// attempt to retrieve the context from database.
    /// <remarks>
    /// This middleware is enabled by <see cref="RouterServiceCollectionExtensions.AddRouter"/> by default.
    /// </remarks>
    public class ContextMiddleware: IPageMiddleware
    {
        private readonly IObjectRetriever _objectRetriever;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// </summary>
        public ContextMiddleware(IObjectRetriever objectRetriever, IServiceProvider serviceProvider)
        {
            _objectRetriever = objectRetriever ?? throw new ArgumentNullException(nameof(objectRetriever));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            var contextType = PageContextSupport.GetContextType(routingInfo.SelectedPageType);
            if (contextType == null)
            {
                return next();
            }

            var context = CreateContextFromUri(routingInfo.SelectedPageType, contextType, routingInfo.Arguments);
            if (context == null)
            {
                return Response.FromStatusCode(404);
            }
            routingInfo.Context = context;

            return next();
        }

        private object CreateContextFromUri(Type pageType, Type contextType, string[] arguments)
        {
            Exception CreateException(string details, Exception inner = null) =>
                new InvalidOperationException(StringsFormatted.ContextMiddleware_CouldNotCreateContext(contextType, pageType, details), inner);

            MethodInfo explicitUriToContext = GetExplicitUriToContext(pageType, contextType);
            if (explicitUriToContext != null)
            {
                try
                {
                    var resolvedArguments = explicitUriToContext
                        .GetParameters()
                        .Skip(1) // the first parameter is string[] args
                        .Select(param => ReflectionHelper.GetParamValue(param, _serviceProvider));

                    return explicitUriToContext.Invoke(null, new object[] { arguments }.Concat(resolvedArguments).ToArray());
                }
                catch (InvalidOperationException e)
                {
                    throw CreateException(
                        StringsFormatted
                            .ContextMiddleware_CouldNotResolveUriToContextDependencies(explicitUriToContext), e);
                }

            }
            // i.e. is database object
            if (typeof(IBindable).IsAssignableFrom(contextType))
            {
                if (arguments.Length != 1)
                {
                    throw CreateException(StringsFormatted.ContextMiddleware_ContextIsDbSoUriShouldHaveOneArgument(contextType));
                }
                return _objectRetriever.GetById(contextType, arguments[0]);
            }

            throw CreateException(StringsFormatted.ContextMiddleware_MarkViewModelAsIBoundOrUriToContext(contextType));
        }

        private MethodInfo GetExplicitUriToContext(Type pageType, Type contextType)
        {
            return ReflectionHelper.GetStaticMethodWithAttribute(pageType, typeof(UriToContextAttribute), typeof(string[]), contextType);
        }
    }
}