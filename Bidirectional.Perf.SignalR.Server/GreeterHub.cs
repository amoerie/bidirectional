using System.Diagnostics;
using System.Text.Json;
using Bidirectional.Perf.SignalR.Contracts;
using Bidirectional.Perf.SignalR.Contracts.Utils;
using Microsoft.AspNetCore.SignalR;

namespace Bidirectional.Perf.SignalR.Server;

public class GreeterHub : Hub<IGreeterClient>, IGreeterHub
{
    private readonly ILogger<GreeterHub> _logger;

    public GreeterHub(ILogger<GreeterHub> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public Task<HelloResponse> ReceiveGreeting(HelloRequest request)
    {
        return Task.FromResult(new HelloResponse("Hello " + request.Name));
    }

    public Task<FileResponse> ReceiveFile(FileRequest request)
    {
        Console.WriteLine($"Received file: {request.Name} with {Math.Round((double)(request.Data.Length / 1024))} KB");

        return Task.FromResult(new FileResponse());
    }

    public async Task<FileResponse> StreamFile(IAsyncEnumerable<byte[]> stream, string fileName)
    {
        using var memoryStream = new MemoryStream();
        await foreach (var chunk in stream)
        {
            await memoryStream.WriteAsync(chunk);
        }
        
        Console.WriteLine($"Received streamed file: {fileName} with {Math.Round((double)(memoryStream.Length / 1024))} KB");
        
        return new FileResponse();
    }

    public Task<DirectoryDto> GetDirectoryInfoAsync(string path, int depth = 3)
    {
        var sw = Stopwatch.StartNew();
        var result = FileManager.GetPath(path, depth);
        sw.Stop();
        _logger.LogInformation("Backend call took {Elapsed}ms", sw.Elapsed.TotalMilliseconds);

        // _logger.LogInformation("Result: {Json}", JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }));
        
        return Task.FromResult(result);
    }
}
