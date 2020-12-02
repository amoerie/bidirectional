using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Demo.Client.GrpcClient
{
    public class ClientServiceConnector : IHostedService
    {
        private readonly GrpcClientFactory _grpcClientFactory;
        private readonly IClientQueuedRequests _clientQueuedRequests;
        private readonly IClientQueuedResponses _clientQueuedResponses;
        private readonly ILogger<ClientServiceConnector> _logger;

        private CancellationTokenSource? _cancellationTokenSource;

        public ClientServiceConnector(GrpcClientFactory grpcClientFactory,
            IClientQueuedRequests clientQueuedRequests,
            IClientQueuedResponses clientQueuedResponses,
            ILogger<ClientServiceConnector> logger)
        {
            _grpcClientFactory = grpcClientFactory ?? throw new ArgumentNullException(nameof(grpcClientFactory));
            _clientQueuedRequests = clientQueuedRequests ?? throw new ArgumentNullException(nameof(clientQueuedRequests));
            _clientQueuedResponses = clientQueuedResponses ?? throw new ArgumentNullException(nameof(clientQueuedResponses));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cancellationTokenSource = null;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("Already started");

            _logger.LogInformation("Starting client service connector");
            
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Run(() => ConnectAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            try
            {

                _logger.LogInformation("Connecting to server");

                var client = _grpcClientFactory.CreateClient<IClientService>(nameof(IClientService));

                _logger.LogInformation("Created client service");

                var responses = SendResponses(cancellationToken);

                _logger.LogInformation("Created responses IAsyncEnumerable");

                var requests
                    = client.ConnectAsync(responses);

                _logger.LogInformation("Connected to server!");

                await ProcessRequests(requests, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to connect to server");
            }
        }

        private async Task ProcessRequests(IAsyncEnumerable<ClientRequest> clientRequests, CancellationToken cancellationToken)
        {
            await foreach (var request in clientRequests.WithCancellation(cancellationToken))
            {
                _logger.LogInformation($"Received request {request.RequestId}");

                await _clientQueuedRequests.WriteAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }

        private async IAsyncEnumerable<ClientResponse> SendResponses([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var response in _clientQueuedResponses.ReadAsync(cancellationToken))
            {
                _logger.LogInformation($"Returning response for request {response.RequestId}");

                yield return response;
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