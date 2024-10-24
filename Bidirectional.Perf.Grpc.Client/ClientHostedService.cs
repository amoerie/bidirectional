using System.Diagnostics;
using Bidirectional.Perf.Grpc.Contracts;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.Grpc.Client;

public class ClientHostedService : IHostedService
{
    private readonly ILogger<ClientHostedService> _logger;
    private readonly IGreeterClientFactory _greeterClientFactory;

    public ClientHostedService(ILogger<ClientHostedService> logger,
         IGreeterClientFactory greeterClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _greeterClientFactory = greeterClientFactory ?? throw new ArgumentNullException(nameof(greeterClientFactory));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
         _logger.LogInformation("Starting up");
         
         await using var greeterClient = _greeterClientFactory.Create();
         await greeterClient.ConnectAsync();
         
         var file = new FileInfo(@"C:\Temp\ct-march.raw");
         if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);
         var fileBytes = await File.ReadAllBytesAsync(file.FullName, cancellationToken);
         var fileRequest = new FileRequest { Name = file.Name, Data = ByteString.CopyFrom(fileBytes) };
         var stopwatch = Stopwatch.StartNew();
         var fileResponse = await greeterClient.SendFile(fileRequest);
         stopwatch.Stop();
        _logger.LogInformation("File sent in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
