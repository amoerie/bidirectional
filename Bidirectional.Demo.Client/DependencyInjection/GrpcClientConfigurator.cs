using System;
using Bidirectional.Demo.Common.Contracts.GetServerProcessInfo;
using Bidirectional.Demo.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.ClientFactory;

namespace Bidirectional.Demo.Client.DependencyInjection
{
    public class GrpcClientConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.AddCodeFirstGrpcClient<IGetServerProcessInfoService>(o =>
            {
                o.Address = new Uri("https://localhost:22377");
            });
        }
    }
}