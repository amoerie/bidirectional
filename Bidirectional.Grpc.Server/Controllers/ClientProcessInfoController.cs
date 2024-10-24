using System;
using System.Threading;
using System.Threading.Tasks;
using Bidirectional.Grpc.Common.Contracts.Client.GetClientProcessInfo;
using Bidirectional.Grpc.Server.GrpcServices.Client.ClientProcessInformation;
using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Grpc.Server.Controllers
{
    [Route("client-process-info")]
    public class ClientProcessInfoController : Controller
    {
        private readonly IGetClientProcessInfoService _getClientProcessInfoService;

        public ClientProcessInfoController(IGetClientProcessInfoService getClientProcessInfoService)
        {
            _getClientProcessInfoService = getClientProcessInfoService ?? throw new ArgumentNullException(nameof(getClientProcessInfoService));
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var getClientProcessInfoRequest = new GetClientProcessInfoRequest();
            var getClientProcessInfoResponse = await _getClientProcessInfoService.GetAsync(getClientProcessInfoRequest, cancellationToken).ConfigureAwait(false);

            return PartialView("_ClientProcessInfo", getClientProcessInfoResponse);
        } 
    }
}