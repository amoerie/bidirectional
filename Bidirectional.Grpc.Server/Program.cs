using System;
using System.Threading;
using Bidirectional.Grpc.Common.Logging;
using Bidirectional.Grpc.Server.GrpcServices;
using Bidirectional.Grpc.Server.GrpcServices.Client;
using Bidirectional.Grpc.Server.GrpcServices.Client.ClientProcessInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using Serilog;

ThreadPool.SetMinThreads(Environment.ProcessorCount * 8, 1000);
ThreadPool.SetMaxThreads(Environment.ProcessorCount * 128, 1000);
                        
Log.Logger = LoggingConfigurator.CreateLogger();

try
{
    Log.ForContext<Program>().Information("Starting up Bidirectional gRPC Server");

    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;

    services.AddWindowsService();
    services.AddControllersWithViews().AddApplicationPart(typeof(Program).Assembly);
    services.AddHttpContextAccessor();
    services.AddSerilog();
    services.AddCodeFirstGrpc();
            
    services.AddHostedService<ClientResponseProcessor>();
    services.AddSingleton<IClientQueuedRequests, ClientQueuedRequests>();
    services.AddSingleton<IClientQueuedResponses, ClientQueuedResponses>();
    services.AddSingleton<IClientPendingRequests, ClientPendingRequests>();
    services.AddSingleton<IClientRequestMetaDataFactory, ClientRequestMetaDataFactory>();
    services.AddSingleton<IClientRequestSender, ClientRequestSender>();
    services.AddSingleton<IGetClientProcessInfoService, GetClientProcessInfoService>();

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
    app.MapGrpcService<GrpcService>();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.ForContext<Program>().Fatal(ex, "Bidirectional gRPC Server exited with an exception");
    throw;
}
finally
{
    Log.ForContext<Program>().Information("Bidirectional gRPC Server is shutting down");
    Log.CloseAndFlush();
}
