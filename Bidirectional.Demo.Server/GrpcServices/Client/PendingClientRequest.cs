using System.Threading.Tasks;
using Bidirectional.Demo.Common.Contracts.Client;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public class PendingClientRequest
    {
        public ClientRequest Request { get; set; }
        public TaskCompletionSource<ClientResponse> Response { get; set; }
        public ClientRequestStatus Status { get; set; }
    }
}