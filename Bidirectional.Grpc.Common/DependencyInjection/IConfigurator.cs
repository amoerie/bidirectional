using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bidirectional.Grpc.Common.DependencyInjection
{
    public interface IConfigurator
    {
        void Configure(HostBuilderContext context, IServiceCollection services);
    }
}