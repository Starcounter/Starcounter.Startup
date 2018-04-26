using System;
using Starcounter.Startup.Abstractions;

namespace Starcounter.Startup
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        public IServiceProvider ApplicationServices { get; set; }
    }
}