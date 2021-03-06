using Bidirectional.Demo.Common.DependencyInjection;
using Bidirectional.Demo.Server.GrpcServices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bidirectional.Demo.Server.DependencyInjection
{
    public class StartupConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            /* Startup */
            /* Each hosted service will be started in the order that they are registered */
            /* Each hosted service must complete its 'StartAsync' call before the next hosted service can start */

            services.AddHostedService<ClientResponseProcessor>();
        }
    }
}