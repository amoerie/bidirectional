using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalR.Demo.Common.DependencyInjection;

namespace SignalR.Demo.Client.DependencyInjection
{
    public class StartupConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            /* Startup */
            /* Each hosted service will be started in the order that they are registered */
            /* Each hosted service must complete its 'StartAsync' call before the next hosted service can start */
        }
    }
}