using System;
using System.Net.Http;
using System.Threading;
using Bidirectional.Demo.Client.GrpcClient;
using Bidirectional.Demo.Client.GrpcClient.GetClientProcessInfo;
using Bidirectional.Demo.Common.Contracts.Client;
using Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo;
using Bidirectional.Demo.Common.DependencyInjection;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.ClientFactory;

namespace Bidirectional.Demo.Client.DependencyInjection
{
    public class GrpcClientConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
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
        }
    }
}