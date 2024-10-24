using System;
using System.Net.Http;
using System.Threading;
using Bidirectional.Grpc.Client.GrpcClient;
using Bidirectional.Grpc.Client.GrpcClient.GetClientProcessInfo;
using Bidirectional.Grpc.Common.Contracts.Client;
using Bidirectional.Grpc.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.ClientFactory;
using Serilog;

ThreadPool.SetMinThreads(Environment.ProcessorCount * 8, 1000);
ThreadPool.SetMaxThreads(Environment.ProcessorCount * 128, 1000);

Log.Logger = LoggingConfigurator.CreateLogger();

try
{
    Log.ForContext<Program>().Information("Starting up Bidirectional gRPC Client");
    
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    
    services.AddSerilog();
    services.AddWindowsService();
    services.AddControllersWithViews().AddApplicationPart(typeof(Program).Assembly);
    services.AddHttpContextAccessor();
    
    services.AddHostedService<ClientServiceConnector>();
    services.AddHostedService<ClientRequestsProcessor>();
    
    services.AddCodeFirstGrpcClient<IGrpcService>(o =>
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60), // This keeps the connection open at all times by pinging every 60s
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true,
        };

        o.Address = new Uri("https://localhost:22377");
        o.ChannelOptionsActions.Add(o =>
        {
            o.HttpClient = null;
            o.HttpHandler = handler;
        });
    });

    services.AddSingleton<IGetClientProcessInfoService, GetClientProcessInfoService>();
    services.AddSingleton<IClientQueuedRequests, ClientQueuedRequests>();
    services.AddSingleton<IClientQueuedResponses, ClientQueuedResponses>();
    services.AddSingleton<IClientRequestProcessor, ClientRequestProcessor>();
    services.AddSingleton<IClientResponseMetaDataFactory, ClientResponseMetaDataFactory>();
    
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/error");
    }
            
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors();
    app.MapDefaultControllerRoute();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.ForContext<Program>().Fatal(ex, "Bidirectional gRPC Client exited with an exception");
    throw;
}
finally
{
    Log.ForContext<Program>().Information("Bidirectional gRPC Client is shutting down");
    Log.CloseAndFlush();
}
