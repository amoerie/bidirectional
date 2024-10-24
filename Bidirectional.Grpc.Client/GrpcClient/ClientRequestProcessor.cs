using System;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Client.GrpcClient.GetClientProcessInfo;
using Bidirectional.Grpc.Common.Contracts.Client;
using Bidirectional.Grpc.Common.Contracts.Client.GetClientProcessInfo;

namespace Bidirectional.Grpc.Client.GrpcClient
{
    public interface IClientRequestProcessor
    {
        Task<ClientResponse> ProcessAsync(ClientRequest clientRequest, CancellationToken cancellationToken);
    }
    
    public class ClientRequestProcessor : IClientRequestProcessor
    {
        private readonly IGetClientProcessInfoService _getClientProcessInfoService;

        public ClientRequestProcessor(
            IGetClientProcessInfoService getClientProcessInfoService)
        {
            _getClientProcessInfoService = getClientProcessInfoService ?? throw new ArgumentNullException(nameof(getClientProcessInfoService));
        }
        
        public async Task<ClientResponse> ProcessAsync(ClientRequest clientRequest, CancellationToken cancellationToken)
        {
            switch (clientRequest)
            {
                case GetClientProcessInfoRequest getClientProcessInfoRequest:
                    return await _getClientProcessInfoService.GetAsync(getClientProcessInfoRequest, cancellationToken).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unsupported client request type: {clientRequest.GetType()}");
            }
        }
    }
}