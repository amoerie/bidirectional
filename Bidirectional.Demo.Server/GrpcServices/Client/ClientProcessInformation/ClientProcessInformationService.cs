using System;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo;

namespace Bidirectional.Demo.Server.GrpcServices.Client.ClientProcessInformation
{
    public interface IGetClientProcessInfoService
    {
        Task<GetClientProcessInfoResponse> GetAsync(GetClientProcessInfoRequest request, CancellationToken cancellationToken);
    }
    
    public class GetClientProcessInfoService : IGetClientProcessInfoService
    {
        private readonly IClientRequestSender _clientRequestSender;

        public GetClientProcessInfoService(IClientRequestSender clientRequestSender)
        {
            _clientRequestSender = clientRequestSender ?? throw new ArgumentNullException(nameof(clientRequestSender));
        }
        
        public Task<GetClientProcessInfoResponse> GetAsync(GetClientProcessInfoRequest request, CancellationToken cancellationToken)
        {
            return _clientRequestSender.SendAsync<GetClientProcessInfoResponse>(request, cancellationToken);
        }
    }
}