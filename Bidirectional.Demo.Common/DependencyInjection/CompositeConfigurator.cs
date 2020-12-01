using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bidirectional.Demo.Common.DependencyInjection
{
    public class CompositeConfigurator : IConfigurator
    {
        private readonly IEnumerable<IConfigurator> _configurators;

        public CompositeConfigurator(IEnumerable<IConfigurator> configurators)
        {
            _configurators = configurators ?? throw new ArgumentNullException(nameof(configurators));
        }
        
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            foreach (var configurator in _configurators)
            {
                configurator.Configure(context, services);
            }
        }
    }
}