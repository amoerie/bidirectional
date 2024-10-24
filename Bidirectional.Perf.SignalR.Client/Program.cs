using Bidirectional.Perf.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.TryAddSingleton<GreeterClient>();
builder.Services.AddHostedService<ClientHostedService>();

var app = builder.Build();

await app.RunAsync();
