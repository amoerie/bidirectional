using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;

namespace Bidirectional.Demo.Client.GrpcClient
{
    public interface IClientQueuedRequests
    {
        ValueTask WriteAsync(ClientRequest clientRequest, CancellationToken cancellationToken);
        IAsyncEnumerable<ClientRequest> ReadAsync(CancellationToken cancellationToken);
    }
    
    public class ClientQueuedRequests : IClientQueuedRequests
    {
        private readonly Channel<ClientRequest> _channel;

        public ClientQueuedRequests()
        {
            _channel = Channel.CreateUnbounded<ClientRequest>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = true
            });
        }

        public ValueTask WriteAsync(ClientRequest clientRequest, CancellationToken cancellationToken)
        {
            if (clientRequest == null) throw new ArgumentNullException(nameof(clientRequest));

            return _channel.Writer.WriteAsync(clientRequest, cancellationToken);
        }

        public async IAsyncEnumerable<ClientRequest> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                while (!cancellationToken.IsCancellationRequested && _channel.Reader.TryRead(out ClientRequest? clientRequest))
                {
                    yield return clientRequest;
                }
            }        
        }
    }
}