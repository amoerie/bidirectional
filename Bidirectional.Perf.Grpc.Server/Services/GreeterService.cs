using Bidirectional.Perf.Grpc.Contracts;
using Grpc.Core;

namespace Bidirectional.Perf.Grpc.Server.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
    
    public override async Task<FileReply> SendFile(IAsyncStreamReader<FileRequest> requestStream, ServerCallContext context)
    {
        var file = await requestStream.MoveNext();
        if (file == false)
        {
            throw new InvalidOperationException("No file received");
        }

        var fileRequest = requestStream.Current;
        _logger.LogInformation("Received file: {FileRequestName} with {Round} KB", fileRequest.Name, Math.Round((double)(fileRequest.Data.Length / 1024)));

        return new FileReply();
    }
}
