using System.Diagnostics;
using System.Text.Json;
using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.SignalR.Client;

public class ClientHostedService : IHostedService
{
    private readonly ILogger<ClientHostedService> _logger;
    private readonly IGreeterClientFactory _greeterClientFactory;

    public ClientHostedService(ILogger<ClientHostedService> logger, IGreeterClientFactory greeterClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _greeterClientFactory = greeterClientFactory ?? throw new ArgumentNullException(nameof(greeterClientFactory));
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting up");

        await using var greeterClient = _greeterClientFactory.Create();
        await greeterClient.ConnectAsync();
        
        for (int i = 0; i < 50; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Getting directory info");
            var info = await greeterClient.GetDirectoryInfoAsync(@"C:\Temp", 25);
            stopwatch.Stop();
            _logger.LogInformation("Success after {Elapsed}ms", stopwatch.Elapsed.TotalMilliseconds);
        }
        
        // _logger.LogInformation("{Json}", infoAsJson);

        // var file = new FileInfo(@"C:\Temp\ct-march.raw");
        // if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);
        //
        // _logger.LogInformation("Sending file");
        // stopwatch = Stopwatch.StartNew();
        // var fileRequest = new FileRequest(file.Name, await File.ReadAllBytesAsync(file.FullName, cancellationToken));
        // var fileResponse = await greeterClient.SendFile(fileRequest);
        // stopwatch.Stop();
        // _logger.LogInformation("File Sent! In {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        //
        //
        // _logger.LogInformation("Streaming file");
        // stopwatch = Stopwatch.StartNew();
        // var fileStreamRequest = new FileRequest(file.Name, await File.ReadAllBytesAsync(file.FullName, cancellationToken));
        // var fileStreamResponse = await greeterClient.StreamFile(fileStreamRequest);
        // stopwatch.Stop();
        // _logger.LogInformation("File Streamed! In {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
