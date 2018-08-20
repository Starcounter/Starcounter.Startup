using System;
using Microsoft.Extensions.Logging;

namespace Starcounter.Startup.Routing
{
    public interface IPageMiddleware
    {
        Response Run(RoutingInfo routingInfo, Func<Response> next);
    }


public class LoggingMiddleware : IPageMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public Response Run(RoutingInfo routingInfo, Func<Response> next)
    {
        _logger.LogInformation("Processing request");
        return next();
    }
}
}