using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client;

namespace Bidirectional.Grpc.Server.GrpcServices.Client
{
    public interface IClientQueuedRequests
    {
        ValueTask<PendingClientRequest> WriteAsync(ClientRequest clientRequest, CancellationToken cancellationToken);
        IAsyncEnumerable<PendingClientRequest> ReadAsync(CancellationToken cancellationToken);
    }
    
    public class ClientQueuedRequests : IClientQueuedRequests
    {
        private readonly Channel<PendingClientRequest> _channel;

        public ClientQueuedRequests()
        {
            _channel = Channel.CreateUnbounded<PendingClientRequest>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public async ValueTask<PendingClientRequest> WriteAsync(ClientRequest clientRequest, CancellationToken cancellationToken)
        {
            if (clientRequest == null) throw new ArgumentNullException(nameof(clientRequest));

            var pendingClientRequest = new PendingClientRequest(
                Request: clientRequest,
                Response: new TaskCompletionSource<ClientResponse>(TaskCreationOptions.RunContinuationsAsynchronously)
            );

            await _channel.Writer.WriteAsync(pendingClientRequest, cancellationToken).ConfigureAwait(false);

            return pendingClientRequest;
        }

        public async IAsyncEnumerable<PendingClientRequest> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                while (!cancellationToken.IsCancellationRequested && _channel.Reader.TryRead(out PendingClientRequest? pendingClientRequest))
                {
                    yield return pendingClientRequest;
                }
            }        
        }
    }
}