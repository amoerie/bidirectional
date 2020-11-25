using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SignalR.Demo.Common.DependencyInjection
{
    public interface IConfigurator
    {
        void Configure(HostBuilderContext context, IServiceCollection services);
    }
}