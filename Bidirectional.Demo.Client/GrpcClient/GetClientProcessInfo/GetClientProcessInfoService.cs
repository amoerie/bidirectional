using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo;

namespace Bidirectional.Demo.Client.GrpcClient.GetClientProcessInfo
{
    public interface IGetClientProcessInfoService
    {
        Task<GetClientProcessInfoResponse> GetAsync(GetClientProcessInfoRequest request, CancellationToken cancellationToken = default);
    }

    public class GetClientProcessInfoService : IGetClientProcessInfoService
    {
        public Task<GetClientProcessInfoResponse> GetAsync(GetClientProcessInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            var currentProcess = Process.GetCurrentProcess();

            var response = new GetClientProcessInfoResponse(currentProcess);

            return Task.FromResult(response);
        }
    }
}