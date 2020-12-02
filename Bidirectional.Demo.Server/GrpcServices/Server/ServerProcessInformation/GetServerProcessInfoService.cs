using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo;

namespace Bidirectional.Demo.Server.GrpcServices.Server.ServerProcessInformation
{
    public class GetServerProcessInfoService : IGetServerProcessInfoService
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