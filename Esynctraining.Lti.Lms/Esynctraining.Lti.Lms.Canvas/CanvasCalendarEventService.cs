using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;

namespace Esynctraining.Lti.Lms.Canvas
{
    public class CanvasCalendarEventService : LmsCalendarEventServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;

        public CanvasCalendarEventService(IEGCEnabledCanvasAPI canvasApi, ILogger logger)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }

        public override async Task<LmsCalendarEventDTO> CreateEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent)
        {
            return await _canvasApi.CreateCalendarEvent(courseId, licenseSettings, lmsEvent);
        }

        public override async Task<LmsCalendarEventDTO> UpdateEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent)
        {
            return await _canvasApi.UpdateCalendarEvent(courseId, licenseSettings, lmsEvent);
        }

        public override async Task<IEnumerable<LmsCalendarEventDTO>> GetUserCalendarEvents(string lmsUserId, Dictionary<string, object> licenseSettings)
        {
            return await _canvasApi.GetUserCalendarEvents(lmsUserId, licenseSettings);
        }

        public override async Task DeleteCalendarEvent(string eventId, Dictionary<string, object> licenseSettings)
        {
            await _canvasApi.DeleteCalendarEvents(Int32.Parse(eventId), licenseSettings);
        }
    }
}
