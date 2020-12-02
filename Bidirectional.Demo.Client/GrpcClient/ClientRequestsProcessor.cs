using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Demo.Client.GrpcClient
{
    public class ClientRequestsProcessor : IHostedService
    {
        private readonly IClientQueuedRequests _clientQueuedRequests;
        private readonly IClientQueuedResponses _clientQueuedResponses;
        private readonly IClientRequestProcessor _clientRequestProcessor;
        private readonly ILogger<ClientRequestsProcessor> _logger;
        private CancellationTokenSource? _cancellationTokenSource;

        public ClientRequestsProcessor(IClientQueuedRequests clientQueuedRequests,
            IClientQueuedResponses clientQueuedResponses,
            IClientRequestProcessor clientRequestProcessor,
            ILogger<ClientRequestsProcessor> logger)
        {
            _clientQueuedRequests = clientQueuedRequests ?? throw new ArgumentNullException(nameof(clientQueuedRequests));
            _clientQueuedResponses = clientQueuedResponses ?? throw new ArgumentNullException(nameof(clientQueuedResponses));
            _clientRequestProcessor = clientRequestProcessor ?? throw new ArgumentNullException(nameof(clientRequestProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cancellationTokenSource = null;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("Already started");

            _logger.LogInformation("Starting client requests processor");

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Run 10 processors simultaneously
            for (var i = 0; i < 10; i++)
            {
                Task.Run(() => ProcessAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }

            return Task.CompletedTask;
        }

        private async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await foreach (var request in _clientQueuedRequests.ReadAsync(cancellationToken))
            {
                _logger.LogInformation($"Processing request with ID '{request.RequestId}'");

                var response = await _clientRequestProcessor.ProcessAsync(request, cancellationToken).ConfigureAwait(false);

                response.RequestId = request.RequestId;
                
                _logger.LogInformation($"Processed request with ID '{request.RequestId}'. Sending response back to server");
                
                await _clientQueuedResponses.WriteAsync(response, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            return Task.CompletedTask;
        }
    }
}