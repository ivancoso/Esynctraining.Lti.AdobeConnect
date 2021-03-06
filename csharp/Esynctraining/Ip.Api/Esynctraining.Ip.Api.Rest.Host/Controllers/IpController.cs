﻿using System;
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
        [HttpPost]
        [ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
        public OperationResultWithData<string> GetUserAgentRemoteAddress()
        {
            _logger.Info($"GetUserAgentRemoteAddress. RemoteIpAddress: {Request.HttpContext.Connection.RemoteIpAddress}");

            return Request.HttpContext.Connection.RemoteIpAddress.ToString().ToSuccessResult();
        }
    }
}
