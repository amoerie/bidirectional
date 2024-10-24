using System.Security.Cryptography.X509Certificates;
using Bidirectional.Perf.SignalR.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.SignalR.Client;

public sealed class GreeterClient : IGreeterClient, IAsyncDisposable
{
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
            {
                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is not HttpClientHandler httpClientHandler)
                    {
                        httpClientHandler = new HttpClientHandler();
                    }
                    httpClientHandler.ClientCertificates.Add(certificate);
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        (httpRequest, serverCertificate, chain, sslPolicyErrors) =>
                        {
                            if (chain is null)
                            {
                                var x509ChainPolicy = new X509ChainPolicy { RevocationMode = X509RevocationMode.NoCheck };
                                chain = new X509Chain { ChainPolicy = x509ChainPolicy };
                            }

                            if (serverCertificate is null)
                            {
                                return false;
                            }

                            var isValidChain = chain.Build(serverCertificate);

                            if (!isValidChain)
                            {
                                for (var index = 0; index < chain.ChainStatus.Length; index++)
                                {
                                    var status = chain.ChainStatus[index];

                                    _logger.LogError(
                                        "Certificate with Subject = {Subject} and Thumbprint = {Thumbprint} chain validation failed: Chain status [{Index}] : {Status} {StatusInformation}",
                                        serverCertificate.SubjectName.Name,
                                        serverCertificate.Thumbprint, index, status.Status, status.StatusInformation);
                                }
                            }

                            return isValidChain;
                        };

                    return httpClientHandler;
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
}
