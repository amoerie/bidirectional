using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    [ServiceContract]
    public interface IClientService
    {
        IAsyncEnumerable<ClientRequest> ReceiveRequestsAsync(CancellationToken cancellationToken);
        Task SendResponsesAsync(IAsyncEnumerable<ClientResponse> responses, CancellationToken cancellationToken);
    }
}