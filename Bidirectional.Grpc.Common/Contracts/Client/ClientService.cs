using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Server.GetServerProcessInfo;
using ProtoBuf.Grpc.Configuration;

namespace Bidirectional.Grpc.Common.Contracts.Client
{
    [Service]
    public interface IGrpcService
    {
        // TODO Alex ReceiveRequestsAsync and SendResponsesAsync could be one single endpoint
        [Operation]
        IAsyncEnumerable<ClientRequest> ReceiveRequestsAsync(CancellationToken cancellationToken);
        
        [Operation]
        Task SendResponsesAsync(IAsyncEnumerable<ClientResponse> responses, CancellationToken cancellationToken);
        
        [Operation]
        Task<GetServerProcessInfoResponse> GetAsync(GetServerProcessInfoRequest request, CancellationToken cancellationToken = default);
    }
}