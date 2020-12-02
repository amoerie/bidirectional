using Bidirectional.Demo.Common.DependencyInjection;
using Bidirectional.Demo.Server.GrpcServices.Client;
using Bidirectional.Demo.Server.GrpcServices.Client.ClientProcessInformation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;

namespace Bidirectional.Demo.Server.DependencyInjection
{
    public class GrpcConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.AddCodeFirstGrpc();
            
            services.AddSingleton<IClientQueuedRequests, ClientQueuedRequests>();
            services.AddSingleton<IClientQueuedResponses, ClientQueuedResponses>();
            services.AddSingleton<IClientPendingRequests, ClientPendingRequests>();
            services.AddSingleton<IClientRequestSender, ClientRequestSender>();
            services.AddSingleton<IGetClientProcessInfoService, GetClientProcessInfoService>();
        }
    }
}