using System;
using System.Text.RegularExpressions;

namespace Starcounter.Startup.Routing.Middleware
{
    public class UriHelper
    {
        private static readonly Regex PartialUriRegex = new Regex($"^/[^/]+{PartialPart}/", RegexOptions.IgnoreCase);
        private static readonly Regex TemplateArgumentRegex = new Regex(Regex.Escape("{?}"));
        private const string PartialPart = "/partial";

        /// <summary>
        /// Converts partial URI to page URI.
        /// </summary>
        /// <param name="partialUri">The partial version of URI, as accessible to the blending engine.</param>
        /// <returns>The page version of URI, as accessible by browser.</returns>
        public static string PartialToPage(string partialUri)
        {
            if (partialUri == null) throw new ArgumentNullException(nameof(partialUri));
            if (partialUri[0] != '/')
            {
                throw new ArgumentException(StringsFormatted.UriHelper_MalformedUri(partialUri), nameof(partialUri));
            }

            var applicationName = ExtractApplicationName(partialUri);
            return "/" + applicationName + partialUri.Substring(applicationName.Length + 1 + PartialPart.Length);
        }

        /// <summary>
        /// Converts page URI to partial URI.
        /// </summary>
        /// <param name="pageUri">The page version of URI, as accessible by browser.</param>
        /// <returns>The partial version of URI, as accessible to the blending engine.</returns>
        public static string PageToPartial(string pageUri)
        {
            if (pageUri == null) throw new ArgumentNullException(nameof(pageUri));
            if (pageUri[0] != '/')
            {
                throw new ArgumentException(StringsFormatted.UriHelper_MalformedUri(pageUri), nameof(pageUri));
            }

            var applicationName = ExtractApplicationName(pageUri);
            return "/" + applicationName + PartialPart + pageUri.Substring(applicationName.Length + 1);
        }

        /// <summary>
        /// Returns true if the supplied URI is a partial URI.
        /// </summary>
        /// <param name="uri">The URI in question</param>
        /// <returns>true if the supplied URI is a partial URI.</returns>
        public static bool IsPartialUri(string uri)
        {
            return PartialUriRegex.IsMatch(uri);
        }

        /// <summary>
        /// Returns the supplied URI with its arguments filled.
        /// </summary>
        /// <param name="uriTemplate">The URI template, with {?} slots to be filled.</param>
        /// <param name="arguments">The array of arguments to fill in the URI template</param>
        /// <returns>The URI template with slots filled.</returns>
        public static string WithArguments(string uriTemplate, params string[] arguments)
        {
            if (uriTemplate == null)
            {
                throw new ArgumentNullException(nameof(uriTemplate));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var slotsCount = TemplateArgumentRegex.Matches(uriTemplate).Count;
            if (slotsCount != arguments.Length)
            {
                throw new ArgumentException(StringsFormatted.UriHelper_CantFillUriTemplateSlotCountMismatch(uriTemplate, arguments, slotsCount));
            }

            foreach (var argument in arguments)
            {
                uriTemplate = TemplateArgumentRegex.Replace(uriTemplate, argument, 1);
            }

            return uriTemplate;
        }

        private static string ExtractApplicationName(string partialUri)
        {
            var indexOfSlash = partialUri.IndexOf("/", 1, StringComparison.Ordinal);
            var applicationName = partialUri.Substring(1, indexOfSlash - 1);
            return applicationName;
        }
    }
}