using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client.GetClientProcessInfo;

namespace Bidirectional.Grpc.Client.GrpcClient.GetClientProcessInfo
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

            var response = currentProcess.ToGetClientProcessInfoResponse();

            return Task.FromResult(response);
        }
    }
}