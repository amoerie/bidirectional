// See https://aka.ms/new-console-template for more information

using Bidirectional.Perf.SignalR.Client;
using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Starting up");

await using var client = new GreeterClient();

await client.ConnectAsync();

Console.WriteLine("Saying hello");

var response = await client.SendGreeting(new HelloRequest("signalR client"));

Console.WriteLine("Received reply: " + response.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();


namespace Bidirectional.Perf.SignalR.Client
{
    public sealed class GreeterClient : IGreeterClient, IAsyncDisposable
    {
        private readonly IGreeterHub _greeterHubProxy;
        private readonly IDisposable _clientRegistration;
        private readonly HubConnection _connection;

        public GreeterClient()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:33666/greeterhub")
                .WithAutomaticReconnect()
                .Build();
            _connection = connection;
            _greeterHubProxy = _connection.ServerProxy<IGreeterHub>();
            _clientRegistration = _connection.ClientRegistration<IGreeterClient>(this);
        }

        public Task ConnectAsync() => _connection.StartAsync();

        public async ValueTask DisposeAsync()
        {
            if (_clientRegistration is IAsyncDisposable registrationAsyncDisposable)
                await registrationAsyncDisposable.DisposeAsync();
            else
                _clientRegistration.Dispose();

            await _connection.DisposeAsync();
        }

        public Task<HelloResponse> SendGreeting(HelloRequest request)
        {
            return _greeterHubProxy.ReceiveGreeting(request);
        }
    }
}
