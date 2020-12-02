using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo
{
    [ServiceContract]
    public interface IGetServerProcessInfoService
    {
        Task<GetServerProcessInfoResponse> GetAsync(GetServerProcessInfoRequest request, CancellationToken cancellationToken = default);
    }
}