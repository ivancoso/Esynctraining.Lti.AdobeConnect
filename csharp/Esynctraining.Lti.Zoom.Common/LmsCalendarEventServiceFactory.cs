using System.Collections.Generic;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Constants;

namespace Esynctraining.Lti.Zoom.Common
{
    public class LmsCalendarEventServiceFactory
    {
        private readonly CanvasCalendarEventService _calendarEventService;

        public LmsCalendarEventServiceFactory(CanvasCalendarEventService calendarEventService)
        {
            _calendarEventService = calendarEventService;
        }

        public LmsCalendarEventServiceBase GetService(int productId, Dictionary<string, object> lmsSettings)
        {
            switch (productId)
            {
                case 1010:
                    bool enableExportToCalendar = (bool)lmsSettings[LmsLicenseSettingNames.EnableCanvasExportToCalendar];
                    if (!enableExportToCalendar)
                        return null;

                    return _calendarEventService;
            }

            return null;
        }
    }
}
