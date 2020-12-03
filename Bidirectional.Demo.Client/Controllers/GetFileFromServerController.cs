using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Demo.Client.Models.Diagnostics;
using Bidirectional.Demo.Common.Contracts.Client;
using Bidirectional.Demo.Common.Contracts.Client.GetFile;
using Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Demo.Client.Controllers
{
    [Route("get-file-server")]
    public class GetFileFromServerController : Controller
    {
        private readonly GrpcClientFactory _grpcClientFactory;

        public GetFileFromServerController(GrpcClientFactory grpcClientFactory)
        {
            _grpcClientFactory = grpcClientFactory ?? throw new ArgumentNullException(nameof(grpcClientFactory));
        }

        [HttpGet("{numberOfBytes?}")]
        public async Task<IActionResult> GetAsync([FromRoute] int? numberOfBytes, CancellationToken cancellationToken)
        {
            var client = _grpcClientFactory.CreateClient<IGrpcService>(nameof(IGrpcService));

            var request = new GetFileRequest()
            {
                Path = "Whatever",
                RequestedSizeInBytes = numberOfBytes
            };

            var stopWatch = Stopwatch.StartNew();

            var getFileFromServerResponse = await client.GetFileAsync(request, cancellationToken).ConfigureAwait(false);

            var model = new DiagnosticModel<GetFileResponse?>(
                getFileFromServerResponse,
                stopWatch.ElapsedMilliseconds,
                getFileFromServerResponse.GenerationTimeInMs,
                getFileFromServerResponse.Bytes.Length);

            return PartialView("_ServerGetFile", model);
        }
    }
}