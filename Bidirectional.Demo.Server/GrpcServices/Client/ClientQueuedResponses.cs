using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public interface IClientQueuedResponses
    {
        ValueTask WriteAsync(ClientResponse clientResponse, CancellationToken cancellationToken);
        IAsyncEnumerable<ClientResponse> ReadAsync(CancellationToken cancellationToken);
    }
    
    public class ClientQueuedResponses : IClientQueuedResponses
    {
        private readonly Channel<ClientResponse> _channel;

        public ClientQueuedResponses()
        {
            _channel = Channel.CreateUnbounded<ClientResponse>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = true
            });
        }

        public ValueTask WriteAsync(ClientResponse clientResponse, CancellationToken cancellationToken)
        {
            if (clientResponse == null) throw new ArgumentNullException(nameof(clientResponse));

            return _channel.Writer.WriteAsync(clientResponse, cancellationToken);
        }

        public async IAsyncEnumerable<ClientResponse> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                while (!cancellationToken.IsCancellationRequested && _channel.Reader.TryRead(out ClientResponse? clientResponse))
                {
                    yield return clientResponse;
                }
            }        
        }
    }
}