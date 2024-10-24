﻿using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Bidirectional.Perf.Grpc.Contracts;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Perf.Grpc.Client;

public class ClientHostedService : IHostedService
{
    private readonly ILogger<ClientHostedService> _logger;

    public ClientHostedService(ILogger<ClientHostedService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting up");

        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var certificate = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, "CN=Client, O=Palex, C=BE", true).First();
        var certificateAwareHandler = new HttpClientHandler();
        certificateAwareHandler.ClientCertificates.Add(certificate);
        certificateAwareHandler.ServerCertificateCustomValidationCallback =
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
        using var channel = GrpcChannel.ForAddress("https://localhost:33658", new GrpcChannelOptions
        {
            HttpHandler = certificateAwareHandler
        });
        var client = new Greeter.GreeterClient(channel);

        _logger.LogInformation("Saying hello");
        var reply = await client.SayHelloAsync(
            new HelloRequest { Name = "GreeterClient" });
        _logger.LogInformation("Received reply: {Message}", reply.Message);
        
        var file = new FileInfo(@"C:\Temp\ct-march.raw");
        if (!file.Exists) throw new InvalidOperationException("Alex you fool, the file I sent you should exist under " + file.FullName);

        _logger.LogInformation("Sending file");
        var fileBytes = await File.ReadAllBytesAsync(file.FullName, cancellationToken);
        var fileRequest = new FileRequest { Name = file.Name, Data = ByteString.CopyFrom(fileBytes) };
        var stopwatch = Stopwatch.StartNew();
        using var fileCall = client.SendFile(cancellationToken: cancellationToken);
        await fileCall.RequestStream.WriteAsync(fileRequest, cancellationToken);
        await fileCall.RequestStream.CompleteAsync();
        var fileReply = await fileCall.ResponseAsync;
        stopwatch.Stop();
        _logger.LogInformation("File sent! In {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
