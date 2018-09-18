using System;

namespace Starcounter.Startup.Routing.Middleware
{
    public class PartialUriHelper : IPartialUriHelper
    {
        private const string PartialPart = "/partial";
        private readonly string _applicationNamePart;


        public PartialUriHelper(IApplicationNameProvider applicationNameProvider)
        {
            _applicationNamePart = "/" + applicationNameProvider.CurrentApplicationName;
        }

        /// <inheritdoc />
        public string PartialToPage(string partialUri)
        {
            return _applicationNamePart + partialUri.Substring(_applicationNamePart.Length + PartialPart.Length);
        }

        /// <inheritdoc />
        public string PageToPartial(string pageUri)
        {
            return _applicationNamePart + PartialPart + pageUri.Substring(_applicationNamePart.Length);
        }

        /// <inheritdoc />
        public bool IsPartialUri(string uri)
        {
            return uri.StartsWith(_applicationNamePart + PartialPart + "/", StringComparison.OrdinalIgnoreCase);
        }
    }
}