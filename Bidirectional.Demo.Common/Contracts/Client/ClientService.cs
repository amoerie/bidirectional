using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client.GetFile;
using Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo;
using ProtoBuf.Grpc.Configuration;

namespace Bidirectional.Demo.Common.Contracts.Client
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

        [Operation]
        Task<GetFileResponse> GetFileAsync(GetFileRequest request, CancellationToken cancellationToken = default);
    }
}