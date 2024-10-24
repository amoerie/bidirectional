using System;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Grpc.Server.GrpcServices.Client
{
    public interface IClientRequestSender
    {
        Task<TResponse> SendAsync<TResponse>(ClientRequest request, CancellationToken cancellationToken)
            where TResponse: ClientResponse;
    }
    
    public class ClientRequestSender : IClientRequestSender
    {
        private readonly IClientQueuedRequests _clientQueuedRequests;
        private readonly IClientRequestMetaDataFactory _clientRequestMetaDataFactory;
        private readonly ILogger<ClientRequestSender> _logger;

        public ClientRequestSender(
            IClientQueuedRequests clientQueuedRequests,
            IClientRequestMetaDataFactory clientRequestMetaDataFactory,
            ILogger<ClientRequestSender> logger)
        {
            _clientQueuedRequests = clientQueuedRequests ?? throw new ArgumentNullException(nameof(clientQueuedRequests));
            _clientRequestMetaDataFactory = clientRequestMetaDataFactory ?? throw new ArgumentNullException(nameof(clientRequestMetaDataFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<TResponse> SendAsync<TResponse>(ClientRequest request, CancellationToken cancellationToken)
            where TResponse: ClientResponse
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request = request with { MetaData = _clientRequestMetaDataFactory.CreateNew() };

            _logger.LogInformation($"Adding request {request.MetaData} to queue");
            
            var pendingClientRequest = await _clientQueuedRequests.WriteAsync(request, cancellationToken).ConfigureAwait(false);

            return (TResponse) await pendingClientRequest.Response.Task.ConfigureAwait(false);
        }
    }
}