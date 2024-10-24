using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client;

namespace Bidirectional.Grpc.Server.GrpcServices.Client
{
    public sealed record PendingClientRequest(
        ClientRequest Request,
        TaskCompletionSource<ClientResponse> Response
    );
}