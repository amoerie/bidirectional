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

        const int timeToRunInSeconds = 60;
        var start = DateTime.UtcNow;

        long requests = 0;

        var tasks = new List<Task>();
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await using var greeterClient = _greeterClientFactory.Create();
                await greeterClient.ConnectAsync();

                var helloRequest = new HelloRequest("I am a Ddosser!");
                
                while (start.AddSeconds(timeToRunInSeconds) > DateTime.UtcNow)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await Parallel.ForAsync(0, 100, cancellationToken, async (_, _) =>
                    {
                        await greeterClient.SendGreeting(helloRequest);
                    });
                    
                    Interlocked.Add(ref requests, 100);
                }
                
            }, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _logger.LogInformation("All done");
        var requestsPerSecond = (double)requests / timeToRunInSeconds;
        _logger.LogInformation("Sent {Requests} requests in {TimeToRunInSeconds} seconds, that's {RequestsPerSecond} requests per second", requests, timeToRunInSeconds, requestsPerSecond);

        // await using var greeterClient = _greeterClientFactory.Create();
        // await greeterClient.ConnectAsync();

        /*await TriggerHelloWorldsAsync(200, greeterClient);

        await TriggerDirListingAsync(50, greeterClient);

        _logger.LogInformation("Going to trigger lots of dir listings and hello worlds together");
        await Task.Delay(100);

        await Task.WhenAll(
            TriggerHelloWorldsAsync(200, greeterClient),
            TriggerDirListingAsync(50, greeterClient)
        );

        _logger.LogInformation("Going to trigger lots of dir listings and hello worlds together with two different clients");
        await Task.Delay(100);

        await using var greeterClient2 = _greeterClientFactory.Create();
        await greeterClient2.ConnectAsync();

        await Task.WhenAll(
            TriggerHelloWorldsAsync(200, greeterClient2),
            TriggerDirListingAsync(50, greeterClient)
        );*/

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

    private async Task TriggerHelloWorldsAsync(int count, GreeterClient greeterClient)
    {
        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < count; i++)
        {
            await greeterClient.SendGreeting(new HelloRequest("Hi number " + i));
        }
        stopwatch.Stop();
        _logger.LogInformation("Sent {Count} hellos in {ElapsedMilliseconds}ms (= {ElapsedPerCall:N2}ms per call)", count, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / count);
    }

    private async Task TriggerDirListingAsync(int count, GreeterClient greeterClient)
    {
        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < count; i++)
        {
            var info = await greeterClient.GetDirectoryInfoAsync(@"C:\Temp", 25);
        }
        stopwatch.Stop();
        _logger.LogInformation("Sent {Count} dir listings in {ElapsedMilliseconds}ms (= {ElapsedPerCall:N2}ms per call)", count, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / count);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
