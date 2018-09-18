namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Provides the name of the currently executing Starcounter application.
    /// </summary>
    public interface IApplicationNameProvider
    {
        /// <summary>
        /// Returns the name of currently executing Starcounter application, as used in URIs.
        /// </summary>
        string CurrentApplicationName { get; }
    }
}