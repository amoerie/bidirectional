using Bidirectional.Demo.Common.DependencyInjection;
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
        }
    }
}