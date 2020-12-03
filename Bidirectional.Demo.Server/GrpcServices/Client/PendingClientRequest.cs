using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public sealed record PendingClientRequest(
        ClientRequest Request,
        TaskCompletionSource<ClientResponse> Response
    );
}