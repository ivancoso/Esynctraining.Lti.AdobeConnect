using System;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Ip.WebApi.Host.Controllers
{
    [Route("my-ip")]
    public class IpController : ControllerBase
    {
        private readonly ILogger _logger;

        public IpController(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
        public OperationResult GetUserAgentRemoteAddress()
        {
            _logger.Info($"GetUserAgentRemoteAddress. RemoteIpAddress: {Request.HttpContext.Connection.RemoteIpAddress}");
            
            return new OperationResult { IsSuccess = true, Message = Request.HttpContext.Connection.RemoteIpAddress.ToString() };
        }
    }
}
