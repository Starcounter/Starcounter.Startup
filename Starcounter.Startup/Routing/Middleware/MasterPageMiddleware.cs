using System;

namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Wraps all responses in a single entity to coordinate blending and common scoping. 
    /// This middleware is enabled by <see cref="RouterServiceCollectionExtensions.AddRouter"/> by default.
    /// </summary>
    public class MasterPageMiddleware: IPageMiddleware
    {
        private readonly Func<RoutingInfo, MasterPageBase> _masterPageFactory;

        public MasterPageMiddleware(Func<RoutingInfo, MasterPageBase> masterPageFactory = null)
        {
            _masterPageFactory = masterPageFactory;
        }

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            if (UriHelper.IsPartialUri(routingInfo.Request.Uri))
            {
                return next();
            }

            Json CreateBlendedResponse() => Self.GET(UriHelper.PageToPartial(routingInfo.Request.Uri));

            if (_masterPageFactory == null)
            {
                return Db.Scope(CreateBlendedResponse);
            }

            var masterPage = _masterPageFactory(routingInfo);
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