using System;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("users")]
    public class UsersController : BaseApiController
    {

        private readonly ZoomMeetingService _meetingService;
        private readonly ZoomUserService _userService;

        public UsersController(ApplicationSettingsProvider settings, 
            ILogger logger, 
            ZoomMeetingService meetingService,
            ZoomUserService userService) : base(settings, logger)
        {
            _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }



    }
}