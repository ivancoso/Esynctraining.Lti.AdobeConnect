using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto.OfficeHours;
using Esynctraining.Lti.Zoom.Common.Services;

namespace Esynctraining.Lti.Zoom.Core
{
    public class EmptyNotificationService : INotificationService {
        public Task<bool> SendOHBookSlotEmail(SlotDto dto, string meetingName, string email, string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendOHCancellationEmail(DateTime date, string meetingName, string message, string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendOHRescheduleEmail(DateTime date, SlotDto dto, string meetingName, string message)
        {
            throw new NotImplementedException();
        }
    }
}