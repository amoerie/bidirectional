using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.SignalR.Client;

public class ClientHostedService : IHostedService
{
    private readonly ILogger<ClientHostedService> _logger;
    private readonly GreeterClient _greeterClient;

    public ClientHostedService(ILogger<ClientHostedService> logger, GreeterClient greeterClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _greeterClient = greeterClient ?? throw new ArgumentNullException(nameof(greeterClient));
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting up");

        await _greeterClient.ConnectAsync();

        _logger.LogInformation("Saying hello");
        var response = await _greeterClient.SendGreeting(new HelloRequest("signalR client"));
        _logger.LogInformation("Received reply: " + response.Message);
        
        
        var file = new FileInfo(@"C:\Temp\ct-march.raw");
        if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);

        var fileRequest = new FileRequest("file", await File.ReadAllBytesAsync(file.FullName, cancellationToken));
        var fileResponse = await _greeterClient.SendFile(fileRequest);
        Console.WriteLine("File sent!");
        
        _logger.LogInformation("Press any key to exit...");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
