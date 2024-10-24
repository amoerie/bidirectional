using MessagePack;

namespace Bidirectional.Perf.SignalR.Contracts;

public interface IGreeterHub
{
    Task<HelloResponse> ReceiveGreeting(HelloRequest request);

    Task<FileResponse> ReceiveFile(FileRequest request);

    Task<FileResponse> StreamFile(IAsyncEnumerable<byte[]> stream, string fileName);
}

public interface IGreeterClient
{
    Task<HelloResponse> SendGreeting(HelloRequest request);

    Task<FileResponse> SendFile(FileRequest request);

    Task<FileResponse> StreamFile(FileRequest request);
}

[MessagePackObject]
public sealed record HelloRequest([property: Key(0)] string Name);

[MessagePackObject]
public sealed record HelloResponse([property: Key(0)] string Message);

[MessagePackObject]
public sealed record FileRequest([property: Key(0)] string Name, [property: Key(1)] byte[] Data);

[MessagePackObject]
public sealed record FileResponse;
