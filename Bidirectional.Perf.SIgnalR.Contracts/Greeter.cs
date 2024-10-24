namespace Bidirectional.Perf.SIgnalR.Contracts;

public interface IGreeterHub
{
    Task<HelloResponse> OnGreet(HelloRequest request);
}

public interface IGreeterClient
{
    Task<HelloResponse> Greet(HelloRequest request);
}

public sealed record HelloRequest(string Name);
public sealed record HelloResponse(string Message);
