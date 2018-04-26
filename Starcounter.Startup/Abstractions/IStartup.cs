using Microsoft.Extensions.DependencyInjection;

namespace Starcounter.Startup.Abstractions
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder applicationBuilder);
    }
}