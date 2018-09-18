namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Provides helper methods wrt partial and page versions of URIs.
    /// All URIs should start with a slash and the application name. Partial URIs
    ///  also have "/partial" attached after the application name.
    /// </summary>
    public interface IPartialUriHelper
    {
        /// <summary>
        /// Converts partial URI to page URI.
        /// </summary>
        /// <param name="partialUri"></param>
        /// <returns></returns>
        string PartialToPage(string partialUri);

        /// <summary>
        /// Converts page URI to partial URI.
        /// </summary>
        /// <param name="pageUri"></param>
        /// <returns></returns>
        string PageToPartial(string pageUri);

        /// <summary>
        /// Returns true if the supplied URI is a partial URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        bool IsPartialUri(string uri);
    }
}