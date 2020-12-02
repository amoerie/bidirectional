using System;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public interface IClientRequestSender
    {
        Task<TResponse> SendAsync<TResponse>(ClientRequest request, CancellationToken cancellationToken)
            where TResponse: ClientResponse;
    }
    
    public class ClientRequestSender : IClientRequestSender
    {
        private readonly IClientQueuedRequests _clientQueuedRequests;
        private readonly ILogger<ClientRequestSender> _logger;
        private int _requestId;

        public ClientRequestSender(IClientQueuedRequests clientQueuedRequests, ILogger<ClientRequestSender> logger)
        {
            _clientQueuedRequests = clientQueuedRequests ?? throw new ArgumentNullException(nameof(clientQueuedRequests));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestId = 0;
        }
        
        public async Task<TResponse> SendAsync<TResponse>(ClientRequest request, CancellationToken cancellationToken)
            where TResponse: ClientResponse
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var requestId = Interlocked.Increment(ref _requestId);
            
            request.RequestId = $"REQ-{requestId}";

            _logger.LogInformation($"Adding request {request.RequestId} to queue");
            
            var pendingClientRequest = await _clientQueuedRequests.WriteAsync(request, cancellationToken).ConfigureAwait(false);

            return (TResponse) await pendingClientRequest.Response.Task.ConfigureAwait(false);
        }
    }
}