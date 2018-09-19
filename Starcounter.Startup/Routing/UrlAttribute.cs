using System;

namespace Starcounter.Startup.Routing
{
    /// <summary>
    /// Apply to a view-model to expose it to users via HTTP or Blending.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UrlAttribute : Attribute
    {
        /// <summary>
        /// The page URI, under which this view-model will be accessible from a browser.
        /// </summary>
        /// <remarks>
        /// Page URI should start with a slash and application name.
        /// </remarks>
        public string Value { get; private set; }

        /// <summary>
        /// If set to false, then the partial URI will not be registered and exposed to the Blending engine. Defaults to true.
        /// </summary>
        public bool Blendable { get; set; } = true;

        /// <summary>
        /// If set to false, then the page URI will not be accessible from a browser. Defaults to true.
        /// </summary>
        public bool External { get; set; } = true;

        public UrlAttribute(string value)
        {
            Value = value;
        }
    }
}