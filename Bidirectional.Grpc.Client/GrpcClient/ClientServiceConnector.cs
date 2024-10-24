using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidirectional.Grpc.Client.GrpcClient
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

            Task.Run(() => ReceiveRequestsAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            Task.Run(() => SendResponsesAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        private async Task ReceiveRequestsAsync(CancellationToken cancellationToken)
        {
            try
            {

                _logger.LogInformation("Connecting to server to receive requests");

                var client = _grpcClientFactory.CreateClient<IGrpcService>(nameof(IGrpcService));

                _logger.LogInformation("Created client service");

                var requests = client.ReceiveRequestsAsync(cancellationToken);

                _logger.LogInformation("Connected to server! Listening for incoming requests...");

                await ProcessRequests(requests, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to connect to server to receive requests");
            }
        }

        private async Task SendResponsesAsync(CancellationToken cancellationToken)
        {
            try
            {

                _logger.LogInformation("Connecting to server to send responses");

                var client = _grpcClientFactory.CreateClient<IGrpcService>(nameof(IGrpcService));

                _logger.LogInformation("Created client service");

                var responses = GetResponses(cancellationToken);

                _logger.LogInformation("Created responses IAsyncEnumerable");

                await client.SendResponsesAsync(responses, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("Connected to server!");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to connect to server to send responses");
            }
        }

        private async Task ProcessRequests(IAsyncEnumerable<ClientRequest> clientRequests, CancellationToken cancellationToken)
        {
            await foreach (var request in clientRequests.WithCancellation(cancellationToken))
            {
                _logger.LogInformation($"Received request {request}");

                await _clientQueuedRequests.WriteAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }

        private async IAsyncEnumerable<ClientResponse> GetResponses([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var response in _clientQueuedResponses.ReadAsync(cancellationToken))
            {
                _logger.LogInformation($"Returning response {response}");

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