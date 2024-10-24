using System.Security.Cryptography.X509Certificates;
using Bidirectional.Perf.SignalR.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSignalR();

// Configure Kestrel to use HTTPS with client certificates
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(33666, listenOptions =>
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
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHub<GreeterHub>("/greeterhub");

app.Run();
