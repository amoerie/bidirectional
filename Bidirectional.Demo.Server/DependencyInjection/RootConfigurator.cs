using System;
using Bidirectional.Demo.Common.DependencyInjection;
using Bidirectional.Demo.Common.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bidirectional.Demo.Server.DependencyInjection
{
    public static class RootConfigurator
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            var configurator = new CompositeConfigurator(
                new IConfigurator[]
                {
                    /* cross cutting concerns */
                    new StartupConfigurator(),
                    
                    /* silos */
                    new WebServerConfigurator(), 
                }
            );

            configurator.Configure(context, services);
        }
    }
}