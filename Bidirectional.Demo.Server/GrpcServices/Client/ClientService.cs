using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public class ClientService : IClientService
    {
        private readonly IClientQueuedRequests _clientQueuedRequests;
        private readonly IClientPendingRequests _clientPendingRequests;
        private readonly IClientQueuedResponses _clientQueuedResponses;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IClientQueuedRequests clientQueuedRequests,
            IClientPendingRequests clientPendingRequests, 
            IClientQueuedResponses clientQueuedResponses,
            ILogger<ClientService> logger)
        {
            _clientQueuedRequests = clientQueuedRequests ?? throw new ArgumentNullException(nameof(clientQueuedRequests));
            _clientPendingRequests = clientPendingRequests ?? throw new ArgumentNullException(nameof(clientPendingRequests));
            _clientQueuedResponses = clientQueuedResponses ?? throw new ArgumentNullException(nameof(clientQueuedResponses));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IAsyncEnumerable<ClientRequest> ReceiveRequestsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Client is connecting to server to start receiving requests");

            return SendRequestsAsync(CancellationToken.None);
        }

        public async Task SendResponsesAsync(IAsyncEnumerable<ClientResponse> responses, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Client is connecting to server to start sending responses");

            await ProcessResponsesAsync(responses, cancellationToken).ConfigureAwait(false);
            
            _logger.LogInformation("Client is disconnecting from server, no more responses to send");
        }

        private async IAsyncEnumerable<ClientRequest> SendRequestsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var queuedRequest in _clientQueuedRequests.ReadAsync(cancellationToken))
            {
                _logger.LogInformation($"Sending request with ID '{queuedRequest.Request.RequestId}' to client");
                
                yield return queuedRequest.Request;
                
                _logger.LogInformation($"Request with ID '{queuedRequest.Request.RequestId}' has been sent to the client");
                
                queuedRequest.Status = ClientRequestStatus.Sent;

                _clientPendingRequests.Write(queuedRequest);
            }
        }

        private async Task ProcessResponsesAsync(IAsyncEnumerable<ClientResponse> responses, CancellationToken cancellationToken = default)
        {
            await foreach (var response in responses.WithCancellation(cancellationToken))
            {
                _logger.LogInformation($"Processing client response for request with ID '{response.RequestId}'");
                
                await _clientQueuedResponses.WriteAsync(response, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}