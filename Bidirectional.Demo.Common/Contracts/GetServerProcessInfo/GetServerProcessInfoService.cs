using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.GetServerProcessInfo
{
    [ServiceContract]
    public interface IGetServerProcessInfoService
    {
        Task<GetServerProcessInfoResponse> GetAsync(GetServerProcessInfoRequest request, CancellationToken cancellationToken = default);
    }
}