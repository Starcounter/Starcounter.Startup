using System;

namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Wraps all responses in a single entity to coordinate blending and common scoping. 
    /// This middleware is enabled by <see cref="RouterServiceCollectionExtensions.AddRouter"/> by default.
    /// </summary>
    public class MasterPageMiddleware: IPageMiddleware
    {
        private readonly Func<MasterPageBase> _masterPageFactory;
        private readonly IApplicationNameProvider _applicationNameProvider;

        public MasterPageMiddleware(IApplicationNameProvider applicationNameProvider,
            Func<MasterPageBase> masterPageFactory = null)
        {
            _masterPageFactory = masterPageFactory;
            _applicationNameProvider = applicationNameProvider;
        }

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            if (UriHelper.IsPartialUri(routingInfo.Request.Uri))
            {
                return next();
            }

            Json CreateBlendedResponse() => Self.GET(UriHelper.PageToPartial(
                routingInfo.Request.Uri,
                _applicationNameProvider.CurrentApplicationName));

            if (_masterPageFactory == null)
            {
                return Db.Scope(CreateBlendedResponse);
            }

            var masterPage = _masterPageFactory();
            if (masterPage == null)
            {
                throw new InvalidOperationException(StringsFormatted.MasterPageMiddleware_MasterPageIsNull());
            }

            var scopedBlendedJson = masterPage.ExecuteInScope(CreateBlendedResponse);
            masterPage.SetPartial(scopedBlendedJson);

            return masterPage;
        }
    }
}