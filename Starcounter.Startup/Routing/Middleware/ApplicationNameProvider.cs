namespace Starcounter.Startup.Routing.Middleware
{
    public class ApplicationNameProvider : IApplicationNameProvider
    {
        /// <inheritdoc />
        public string CurrentApplicationName => Application.Current.Name;
    }
}