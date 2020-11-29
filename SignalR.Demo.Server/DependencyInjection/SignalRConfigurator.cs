using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalR.Demo.Common.DependencyInjection;

namespace SignalR.Demo.Server.DependencyInjection
{
    public class SignalRConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}