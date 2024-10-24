using Bidirectional.Perf.SignalR.Server;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHub<GreeterHub>("/greeterhub");

app.Run();
