using System;

namespace Starcounter.Startup.Abstractions
{
    public interface IStartupFilter
    {
        Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next);
    }
}