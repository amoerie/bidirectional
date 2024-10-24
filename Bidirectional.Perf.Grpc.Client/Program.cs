using Bidirectional.Perf.Grpc.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ClientHostedService>();

var app = builder.Build();

await app.RunAsync();
