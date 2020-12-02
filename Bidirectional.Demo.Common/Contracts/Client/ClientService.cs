using System.Collections.Generic;
using System.Threading;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    public interface IClientService
    {
        IAsyncEnumerable<ClientRequest> ConnectAsync(IAsyncEnumerable<ClientResponse> responses);
    }
}