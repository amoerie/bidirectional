using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.SignalR.Client;

public interface IGreeterClientFactory
{
    GreeterClient Create();
}

public class GreeterClientFactory(ILoggerFactory loggerFactory) : IGreeterClientFactory
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    public GreeterClient Create()
    {
        var logger = _loggerFactory.CreateLogger<GreeterClient>();
        return new GreeterClient(logger);
    }
}
