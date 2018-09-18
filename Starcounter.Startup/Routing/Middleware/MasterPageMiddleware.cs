using System;

namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Wraps all responses in a single entity to coordinate blending and common scoping. 
    /// This middleware is enabled by <see cref="RouterServiceCollectionExtensions.AddRouter"/> by default.
    /// </summary>
    public class MasterPageMiddleware: IPageMiddleware
    {
        private readonly IPartialUriHelper _partialUriHelper;
        private readonly Func<MasterPageBase> _masterPageFactory;

        public MasterPageMiddleware(IPartialUriHelper partialUriHelper, Func<MasterPageBase> masterPageFactory = null)
        {
            _partialUriHelper = partialUriHelper ?? throw new ArgumentNullException(nameof(partialUriHelper));
            _masterPageFactory = masterPageFactory;
        }

        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            if (_partialUriHelper.IsPartialUri(routingInfo.Request.Uri))
            {
                return next();
            }

            Json CreateBlendedResponse() => Self.GET( // enable blending
                _partialUriHelper.PageToPartial(routingInfo.Request.Uri));

            if (_masterPageFactory == null)
            {
                return Db.Scope(CreateBlendedResponse); // put all the blended view-models in a common scope
            }

            var masterPage = _masterPageFactory();
            if (masterPage == null)
            {
                throw new InvalidOperationException(StringsFormatted.MasterPageMiddleware_MasterPageIsNull());
            }

            var scopedBlendedJson = masterPage.ExecuteInScope(CreateBlendedResponse);
            masterPage.SetBlended(scopedBlendedJson);

            return masterPage;
        }
    }
}