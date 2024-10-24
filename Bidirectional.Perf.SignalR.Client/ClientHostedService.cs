using System.Diagnostics;
using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.SignalR.Client;

public class ClientHostedService : IHostedService
{
    private readonly ILogger<ClientHostedService> _logger;
    private readonly IGreeterClientFactory _greeterClientFactory;
    private readonly GreeterClient _greeterClient;

    public ClientHostedService(ILogger<ClientHostedService> logger, IGreeterClientFactory greeterClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _greeterClientFactory = greeterClientFactory ?? throw new ArgumentNullException(nameof(greeterClientFactory));
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting up");
        
        // Number of parallel requests
        int numberOfConnections = 25;
        
        // Number of requests per connection
        int requestsPerConnection = 400;
        
        List<Task> tasks = new List<Task>();

        var stopwatch = Stopwatch.StartNew();
            
        // Launch multiple connections (parallel tasks)
        for (int i = 0; i < numberOfConnections; i++)
        {
            var iCopy = i;
            tasks.Add(Task.Run(async () =>
            {
                await using var greeterClient = _greeterClientFactory.Create();
                
                await greeterClient.ConnectAsync();
                
                for (int j = 0; j < requestsPerConnection; j++)
                {
                    try
                    {
                        var jCopy = j;
                        var reply = await greeterClient.SendGreeting(new HelloRequest($"Client {iCopy}-{jCopy}"));
                        // _logger.LogInformation("Response: {ReplyMessage}", reply.Message);
                        
                        // var file = new FileInfo(@"C:\Temp\ct-march.raw");
                        // if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);
                        // var fileRequest = new FileRequest("file", await File.ReadAllBytesAsync(file.FullName, cancellationToken));
                        // var fileResponse = await _greeterClient.SendFile(fileRequest);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending greeting");
                    }
                }
            }, cancellationToken));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
        
        stopwatch.Stop();

        // Output the results
        var numberOfRequests = numberOfConnections * requestsPerConnection;
        _logger.LogInformation("Completed {NumberOfRequests} requests over {NumberOfConnections} connections in {StopwatchElapsed}", numberOfRequests, numberOfConnections, stopwatch.Elapsed);
        _logger.LogInformation("Average time per request: {ElapsedMilliseconds} ms", (double) stopwatch.ElapsedMilliseconds / numberOfRequests);
        
        var file = new FileInfo(@"C:\Temp\ct-march.raw");
        if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);
        
        _logger.LogInformation("Sending file");
        stopwatch = Stopwatch.StartNew();
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

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
