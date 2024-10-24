namespace Bidirectional.Perf.SignalR.Contracts;

public interface IGreeterHub
{
    Task<HelloResponse> ReceiveGreeting(HelloRequest request);
    
    Task<FileResponse> ReceiveFile(FileRequest request);
}

public interface IGreeterClient
{
    Task<HelloResponse> SendGreeting(HelloRequest request);

    Task<FileResponse> SendFile(FileRequest request);
}

public sealed record HelloRequest(string Name);
public sealed record HelloResponse(string Message);

public sealed record FileRequest(string Name, byte[] Data);
public sealed record FileResponse;
