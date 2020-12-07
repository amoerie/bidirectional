using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Client.Models;
using Bidirectional.Demo.Common.Contracts.Client;
using Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Demo.Client.Controllers
{
    [Route("ping-pong")]
    public class PingPongController : Controller
    {
        private readonly GrpcClientFactory _grpcClientFactory;

        public PingPongController(GrpcClientFactory grpcClientFactory)
        {
            _grpcClientFactory = grpcClientFactory ?? throw new ArgumentNullException(nameof(grpcClientFactory));
        }

        [HttpGet("")]
        public async Task<IActionResult> PingPongTest(CancellationToken cancellationToken)
        {
            var client = _grpcClientFactory.CreateClient<IGrpcService>(nameof(IGrpcService));

            var numberOfRequests = 10000;

            var stopwatch = Stopwatch.StartNew();

            Enumerable.Range(1, numberOfRequests).AsParallel().WithDegreeOfParallelism(128).ForAll(async x =>
            {
                var response = await client.Ping(cancellationToken).ConfigureAwait(false);
            });

            stopwatch.Stop();

            return PartialView("_PingPongTest", new PingPongResult(numberOfRequests, (int)stopwatch.ElapsedMilliseconds));
        } 
    }
}