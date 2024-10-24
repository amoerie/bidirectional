using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.SignalR.Client;

public sealed class GreeterClient : IGreeterClient, IAsyncDisposable
{
    private const int ChunkSize = 265 * 1024; //1KB
    private readonly ILogger<GreeterClient> _logger;
    private IGreeterHub? _greeterHubProxy;
    private IDisposable? _clientRegistration;
    private HubConnection? _connection;

    public GreeterClient(ILogger<GreeterClient> logger)
    {
        _logger = logger;
    }

    public async Task ConnectAsync()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, "CN=Client, O=Palex, C=BE", true).First();
        
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:33666/greeterhub", options =>
            // .WithUrl("https://192.168.0.122:33666/greeterhub", options =>
            {
                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is not SocketsHttpHandler socketsHttpHandler)
                    {
                        socketsHttpHandler = new SocketsHttpHandler();
                    }

                    socketsHttpHandler.SslOptions = new SslClientAuthenticationOptions
                    {
                        ClientCertificates = new X509Certificate2Collection { certificate },
                        RemoteCertificateValidationCallback = (_, serverCertificate, chain, _) =>
                        {
                            if (chain is null)
                            {
                                var x509ChainPolicy = new X509ChainPolicy
                                    { RevocationMode = X509RevocationMode.NoCheck };
                                chain = new X509Chain { ChainPolicy = x509ChainPolicy };
                            }

                            if (serverCertificate is null ||
                                serverCertificate is not X509Certificate2 serverCertificate2)
                            {
                                return false;
                            }

                            var isValidChain = chain.Build(serverCertificate2);

                            if (!isValidChain)
                            {
                                for (var index = 0; index < chain.ChainStatus.Length; index++)
                                {
                                    var status = chain.ChainStatus[index];

                                    _logger.LogError(
                                        "Certificate with Subject = {Subject} and Thumbprint = {Thumbprint} chain validation failed: Chain status [{Index}] : {Status} {StatusInformation}",
                                        serverCertificate2.SubjectName.Name,
                                        serverCertificate2.Thumbprint, index, status.Status, status.StatusInformation);
                                }
                            }

                            return isValidChain;
                        }
                    };

                    return socketsHttpHandler;
                };
            })
            .WithAutomaticReconnect()
            .Build();
        _connection = connection;
        _greeterHubProxy = _connection.ServerProxy<IGreeterHub>();
        _clientRegistration = _connection.ClientRegistration<IGreeterClient>(this);
        await  _connection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_clientRegistration is IAsyncDisposable registrationAsyncDisposable)
            await registrationAsyncDisposable.DisposeAsync();
        else
            _clientRegistration?.Dispose();

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    public Task<HelloResponse> SendGreeting(HelloRequest request)
    {
        if (_greeterHubProxy is null)
        {
            throw new InvalidOperationException("Not connected yet");
        }
        
        return _greeterHubProxy.ReceiveGreeting(request);
    }

    public Task<FileResponse> SendFile(FileRequest request)
    {
        if (_greeterHubProxy is null)
        {
            throw new InvalidOperationException("Not connected yet");
        }

        return _greeterHubProxy.ReceiveFile(request);
    }

    public Task<FileResponse> StreamFile(FileRequest request)
    {
        if (_greeterHubProxy is null)
        {
            throw new InvalidOperationException("Not connected yet");
        }

        return _greeterHubProxy.StreamFile(StreamByteArray(request.Data, ChunkSize), request.Name);
    }
    
    private static async IAsyncEnumerable<byte[]> StreamByteArray(byte[] byteArray, int chunkSize)
    {
        for (var i = 0; i < byteArray.Length; i += chunkSize)
        {
            var length = Math.Min(chunkSize, byteArray.Length - i);
            var chunk = new byte[length];
            Array.Copy(byteArray, i, chunk, 0, length);
            yield return chunk;

            // Optional: Simulate some delay for demonstration purposes
            // await Task.Delay(100); 
            await Task.Yield();
        }
    }

}
