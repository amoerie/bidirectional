using System.Diagnostics;
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
        _logger.LogInformation("Received reply: {Message}", response.Message);
        
        
        var file = new FileInfo(@"C:\Temp\ct-march.raw");
        if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);

        _logger.LogInformation("Sending file");
        var stopwatch = Stopwatch.StartNew();
        var fileRequest = new FileRequest(file.Name, await File.ReadAllBytesAsync(file.FullName, cancellationToken));
        var fileResponse = await _greeterClient.SendFile(fileRequest);
        stopwatch.Stop();
        _logger.LogInformation("File Sent! In {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        
        
        _logger.LogInformation("Streaming file");
        stopwatch = Stopwatch.StartNew();
        var fileStreamRequest = new FileRequest(file.Name, await File.ReadAllBytesAsync(file.FullName, cancellationToken));
        var fileStreamResponse = await _greeterClient.StreamFile(fileStreamRequest);
        stopwatch.Stop();
        _logger.LogInformation("File Streamed! In {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        
        _logger.LogInformation("Press any key to exit...");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
