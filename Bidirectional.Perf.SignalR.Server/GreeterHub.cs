using Bidirectional.Perf.SIgnalR.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Bidirectional.Perf.SignalR.Server;

public class GreeterHub : Hub<IGreeterClient>, IGreeterHub
{
    public Task<HelloResponse> OnGreet(HelloRequest request)
    {
        return Task.FromResult<HelloResponse>(new HelloResponse("Hello " + request.Name));
    }
}
