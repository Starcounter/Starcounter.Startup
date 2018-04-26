using System;

namespace Starcounter.Startup.Abstractions
{
    public interface IApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; set; }
    }
}