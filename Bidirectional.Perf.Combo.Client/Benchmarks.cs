using BenchmarkDotNet.Attributes;
using Bidirectional.Perf.Grpc.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.Combo.Client;

[MemoryDiagnoser]
[ShortRunJob]
public class Benchmarks
{
    private ServiceProvider? _serviceProvider;
    private Bidirectional.Perf.Grpc.Client.IGreeterClientFactory? _rpcClientFactory;
    private Bidirectional.Perf.SignalR.Client.IGreeterClientFactory? _signalClientFactory;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var services = new ServiceCollection();
        services.AddLogging(l =>
        {
            l.ClearProviders();
            l.SetMinimumLevel(LogLevel.Critical);
        });
        services
            .AddSingleton<Bidirectional.Perf.SignalR.Client.IGreeterClientFactory,
                Bidirectional.Perf.SignalR.Client.GreeterClientFactory>();
        services
            .AddSingleton<Bidirectional.Perf.Grpc.Client.IGreeterClientFactory,
                Bidirectional.Perf.Grpc.Client.GreeterClientFactory>();

        _serviceProvider = services.BuildServiceProvider();

        _rpcClientFactory = _serviceProvider.GetRequiredService<Bidirectional.Perf.Grpc.Client.IGreeterClientFactory>();
        _signalClientFactory =
            _serviceProvider.GetRequiredService<Bidirectional.Perf.SignalR.Client.IGreeterClientFactory>();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _serviceProvider?.Dispose();
    }
    
    [Params(25)]
    public int NumberOfConnections { get; set; }
    
    [Params(400)]
    public int NumberOfRequestsPerConnection { get; set; }

    [Benchmark(Description = "gRPC")]
    public async Task Grpc()
    {
        await Parallel.ForAsync(0, NumberOfConnections, async (connection, connectionToken) =>
        {
            await using var client = _rpcClientFactory!.Create();
            await client.ConnectAsync();
            await Parallel.ForAsync(0, NumberOfRequestsPerConnection, connectionToken, async (request, requestToken) =>
            {
                await client.SayHello(new HelloRequest { Name = "Benchmark " + request });
            });
        });
    }

    [Benchmark(Description = "SignalR")]
    public async Task Signalr()
    {
        await Parallel.ForAsync(0, NumberOfConnections, async (connection, connectionToken) =>
        {
            await using var client = _signalClientFactory!.Create();
            await client.ConnectAsync();
            await Parallel.ForAsync(0, NumberOfRequestsPerConnection, connectionToken, async (request, requestToken) =>
            {
                await client.SendGreeting(new SignalR.Contracts.HelloRequest("Benchmark " + request));
            });
        });
    }
}
