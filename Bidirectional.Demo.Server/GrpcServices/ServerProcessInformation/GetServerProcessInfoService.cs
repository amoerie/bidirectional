using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Server.GrpcServices.ServerProcessInformation
{
    [ServiceContract]
    public interface IGetServerProcessInfoService
    {
        Task<GetServerProcessInfoResponse> GetAsync(GetServerProcessInfoRequest request, CancellationToken cancellationToken = default);
    }
    
    public class GetGetServerProcessInfoService : IGetServerProcessInfoService
    {
        public Task<GetServerProcessInfoResponse> GetAsync(GetServerProcessInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var currentProcess = Process.GetCurrentProcess();

            var response = new GetServerProcessInfoResponse(currentProcess);

            return Task.FromResult(response);
        }
    }
}