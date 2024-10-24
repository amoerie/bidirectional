namespace Bidirectional.Perf.SignalR.Contracts;

public interface IGreeterHub
{
    Task<HelloResponse> ReceiveGreeting(HelloRequest request);
}

public interface IGreeterClient
{
    Task<HelloResponse> SendGreeting(HelloRequest request);
}

public sealed record HelloRequest(string Name);
public sealed record HelloResponse(string Message);
