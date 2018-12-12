using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;

namespace Esynctraining.Lti.Zoom.Common
{
    public class LmsCalendarEventServiceFactory
    {
        private readonly CanvasCalendarEventService _calendarEventService;

        public LmsCalendarEventServiceFactory(CanvasCalendarEventService calendarEventService)
        {
            _calendarEventService = calendarEventService;
        }

        public LmsCalendarEventServiceBase GetService(int productId)
        {
            switch (productId)
            {
                case 1010:
                    return _calendarEventService;
            }

            return null;
        }
    }
}
