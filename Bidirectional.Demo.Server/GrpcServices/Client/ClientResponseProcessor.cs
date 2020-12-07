using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public sealed class ClientResponseProcessor : IHostedService, IDisposable
    {
        private readonly IClientPendingRequests _clientPendingRequests;
        private readonly IClientQueuedResponses _clientQueuedResponses;
        private readonly ILogger<ClientResponseProcessor> _logger;
        private CancellationTokenSource? _cancellationTokenSource;

        public ClientResponseProcessor(IClientPendingRequests clientPendingRequests,
            IClientQueuedResponses clientQueuedResponses,
            ILogger<ClientResponseProcessor> logger)
        {
            _clientPendingRequests = clientPendingRequests ?? throw new ArgumentNullException(nameof(clientPendingRequests));
            _clientQueuedResponses = clientQueuedResponses ?? throw new ArgumentNullException(nameof(clientQueuedResponses));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cancellationTokenSource = null;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("Already started");
            
            _logger.LogInformation("Starting clients responses processor");
            
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Run(() => ProcessClientResponsesAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            
            return Task.CompletedTask;
        }

        [SuppressMessage("ReSharper", "CA1031")]
        private async Task ProcessClientResponsesAsync(CancellationToken cancellationToken)
        {
            await foreach (var clientResponse in _clientQueuedResponses.ReadAsync(cancellationToken))
            {
                try
                {
                    ProcessClientResponse(clientResponse);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Failed to process client response");
                }
            }
        }

        private void ProcessClientResponse(ClientResponse? clientResponse)
        {
            if (clientResponse == null)
            {
                _logger.LogWarning($"NULL response in queued client responses");
                return;
            }
                
            if (clientResponse.MetaData.Equals(ClientResponseMetaData.Missing))
            {
                _logger.LogWarning($"Response {clientResponse} does not have meta data");
                return;
            }
            
            _logger.LogInformation($"Processing response for request '{clientResponse.MetaData.RequestId}'");
                
            if (!_clientPendingRequests.TryRead(clientResponse.MetaData.RequestId, out var pendingClientRequest))
            {
                _logger.LogWarning($"Response {clientResponse} with ID '{clientResponse.MetaData.RequestId}' does not have a matching pending request");
                return;
            }
                
            if (pendingClientRequest == null)
            {
                _logger.LogWarning($"NULL request in pending client requests, request ID was '{clientResponse.MetaData.RequestId}'");
                return;
            }

            pendingClientRequest.Response.SetResult(clientResponse);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {   
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}