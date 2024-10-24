using System.Security.Cryptography.X509Certificates;
using Bidirectional.Perf.Grpc.Server.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 1_000_000_000; // 1GB
});

// Configure Kestrel to use HTTPS with client certificates
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(33658, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(httpsOptions =>
        {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certificate = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, "CN=Server, O=Palex, C=BE", true).First();
            
            httpsOptions.ServerCertificate = certificate;
            httpsOptions.CheckCertificateRevocation = false;
            httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        });
    });
    
    options.Limits.MaxRequestBodySize = 1_000_000_000; // 1GB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
