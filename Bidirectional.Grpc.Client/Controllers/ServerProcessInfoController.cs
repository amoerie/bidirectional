using System;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client;
using Bidirectional.Grpc.Common.Contracts.Server.GetServerProcessInfo;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Grpc.Client.Controllers
{
    [Route("server-process-info")]
    public class ServerProcessInfoController : Controller
    {
        private readonly GrpcClientFactory _grpcClientFactory;

        public ServerProcessInfoController(GrpcClientFactory grpcClientFactory)
        {
            _grpcClientFactory = grpcClientFactory ?? throw new ArgumentNullException(nameof(grpcClientFactory));
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var client = _grpcClientFactory.CreateClient<IGrpcService>(nameof(IGrpcService));
            
            var getServerProcessInfoResponse = await client.GetAsync(new GetServerProcessInfoRequest(), cancellationToken).ConfigureAwait(false);

            return PartialView("_ServerProcessInfo", getServerProcessInfoResponse);
        } 
    }
}