using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Bidirectional.Perf.SignalR.Server;

public class GreeterHub : Hub<IGreeterClient>, IGreeterHub
{
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
}
