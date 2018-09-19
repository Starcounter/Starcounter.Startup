using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Starcounter.Startup.Routing.Activation;
using Starcounter.Startup.Routing.Middleware;

namespace Starcounter.Startup.Routing
{
    public class Router : IRouter
    {
        private readonly IPageCreator _pageCreator;
        private readonly ILogger<Router> _logger;
        private readonly List<IPageMiddleware> _middleware;

        public Router(IPageCreator pageCreator,
            IEnumerable<IPageMiddleware> middlewares,
            ILogger<Router> logger)
        {
            _pageCreator = pageCreator;
            _logger = logger;
            _middleware = middlewares.ToList();
        }

        /// <inheritdoc />
        public void HandleGet(Type pageType, HandlerOptions handlerOptions = null)
        {
            var urlAttributes = pageType.GetCustomAttributes<UrlAttribute>().ToList();
            if (!urlAttributes.Any())
            {
                throw new ArgumentException(StringsFormatted.Router_TypeHasNoUrlAttribute(pageType), nameof(pageType));
            }

            foreach (var urlAttribute in urlAttributes)
            {
                var pageUri = urlAttribute.Value;
                if (urlAttribute.External)
                {
                    HandleGet(pageUri, pageType, handlerOptions);
                }
                if (urlAttribute.Blendable)
                {
                    HandleGet(UriHelper.PageToPartial(pageUri),
                        pageType,
                        SelfOnlyHandlerOptions(handlerOptions));
                }
            }
        }

        /// <inheritdoc />
        public void HandleGet(string url, Type pageType, HandlerOptions handlerOptions = null)
        {
            _logger.LogInformation(StringsFormatted.Router_RegisteringUri(url, pageType));
            var argumentsNo = Regex.Matches(url, @"\{\?\}").Count;
            switch (argumentsNo)
            {
                case 0:
                    Handle.GET(url,
                        (Request request) => RunResponse(pageType, request),
                        handlerOptions);
                    break;
                case 1:
                    Handle.GET<string, Request>(url,
                        (arg, request) => RunResponse(pageType, request, arg),
                        handlerOptions);
                    break;
                case 2:
                    Handle.GET<string, string, Request>(url,
                        (arg1, arg2, request) => RunResponse(pageType, request, arg1, arg2),
                        handlerOptions);
                    break;
                case 3:
                    Handle.GET<string, string, string, Request>(url,
                        (arg1, arg2, arg3, request) => RunResponse(pageType, request, arg1, arg2, arg3),
                        handlerOptions);
                    break;
                case 4:
                    Handle.GET<string, string, string, string, Request>(url,
                        (arg1, arg2, arg3, arg4, request) => RunResponse(pageType, request, arg1, arg2, arg3, arg4),
                        handlerOptions);
                    break;
                default:
                    throw new NotSupportedException(Strings.Router_MoreParametersNotSupported);
            }
        }

        private HandlerOptions SelfOnlyHandlerOptions(HandlerOptions original)
        {
            if (original == null)
            {
                return new HandlerOptions() {SelfOnly = true};
            }

            original.SelfOnly = true;
            return original;
        }

        private Response RunResponse(Type pageType, Request request, params string[] arguments)
        {
            var routingInfo = new RoutingInfo { Request = request, SelectedPageType = pageType, Arguments = arguments };
            return RunWithMiddleware(
                routingInfo,
                _middleware.Concat(new[] { new TerminalMiddleware(() => _pageCreator.Create(routingInfo)) }));
        }

        private Response RunWithMiddleware(RoutingInfo routingInfo, IEnumerable<IPageMiddleware> middlewares)
        {
            // todo enumerate only once
            // the last 'middleware' is actually just CreateAndInitPage which will always ignore the empty list of middleware it gets
            var pageMiddleware = middlewares.First();
            return pageMiddleware.Run(routingInfo, () => RunWithMiddleware(routingInfo, middlewares.Skip(1)));
        }

        private class TerminalMiddleware : IPageMiddleware
        {
            private readonly Func<Response> _creator;

            public TerminalMiddleware(Func<Response> creator)
            {
                _creator = creator;
            }

            public Response Run(RoutingInfo routingInfo, Func<Response> next)
            {
                return _creator();
            }
        }
    }
}