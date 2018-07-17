using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto.OfficeHours;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class EmptyNotificationService : INotificationService {
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