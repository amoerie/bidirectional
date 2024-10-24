using MessagePack;

namespace Bidirectional.Perf.SignalR.Contracts;

public interface IGreeterHub
{
    Task<HelloResponse> ReceiveGreeting(HelloRequest request);

    Task<FileResponse> ReceiveFile(FileRequest request);

    Task<FileResponse> StreamFile(IAsyncEnumerable<byte[]> stream, string fileName);
    Task<DirectoryDto> GetDirectoryInfoAsync(string path, int depth = 3);
}

public interface IGreeterClient
{
    Task<HelloResponse> SendGreeting(HelloRequest request);

    Task<FileResponse> SendFile(FileRequest request);

    Task<FileResponse> StreamFile(FileRequest request);
    
    Task<DirectoryDto> GetDirectoryInfoAsync(string path, int depth = 3);
}

[MessagePackObject]
public sealed record HelloRequest([property: Key(0)] string Name);

[MessagePackObject]
public sealed record HelloResponse([property: Key(0)] string Message);

[MessagePackObject]
public sealed record FileRequest([property: Key(0)] string Name, [property: Key(1)] byte[] Data);

[MessagePackObject]
public sealed record FileResponse;
