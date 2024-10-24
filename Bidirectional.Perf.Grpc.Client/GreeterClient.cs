using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Bidirectional.Perf.Grpc.Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.Grpc.Client;

public class GreeterClient : IAsyncDisposable
{
    private readonly ILogger<GreeterClient> _logger;
    private Greeter.GreeterClient? _client;
    private GrpcChannel? _channel;

    public GreeterClient(ILogger<GreeterClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ConnectAsync()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates
            .Find(X509FindType.FindBySubjectDistinguishedName, "CN=Client, O=Palex, C=BE", true).First();
        var certificateAwareHandler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                ClientCertificates = new X509Certificate2Collection { certificate },
                RemoteCertificateValidationCallback = (_, serverCertificate, chain, _) =>
                {
                    if (chain is null)
                    {
                        var x509ChainPolicy = new X509ChainPolicy { RevocationMode = X509RevocationMode.NoCheck };
                        chain = new X509Chain { ChainPolicy = x509ChainPolicy };
                    }

                    if (serverCertificate is null || serverCertificate is not X509Certificate2 serverCertificate2)
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
            }
        };
        _channel = GrpcChannel.ForAddress("https://localhost:33658", new GrpcChannelOptions
        // _channel = GrpcChannel.ForAddress("https://192.168.0.122:33658", new GrpcChannelOptions
        {
            HttpHandler = certificateAwareHandler,
        });
        _client = new Greeter.GreeterClient(_channel);

        await _channel.ConnectAsync();
    }

    public async Task<HelloReply> SayHello(HelloRequest helloRequest)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("Client is not connected");
        }

        return await _client.SayHelloAsync(helloRequest);
    }
    
    public async Task<FileReply> SendFile(FileRequest fileRequest)
    {
        if (_client is null)
        {
            throw new InvalidOperationException("Client is not connected");
        }

        using var call = _client.SendFile();
        await call.RequestStream.WriteAsync(fileRequest);
        await call.RequestStream.CompleteAsync();
        return await call.ResponseAsync;
    }

    public ValueTask DisposeAsync()
    {
        _channel?.Dispose();
        return default;
    }
}
