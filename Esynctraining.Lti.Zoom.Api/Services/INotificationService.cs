using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto.OfficeHours;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface INotificationService
    {
        Task<bool> SendOHCancellationEmail(DateTime date, string meetingName, string message, string email);
        Task<bool> SendOHRescheduleEmail(DateTime date, SlotDto dto, string meetingName, string message);
    }
}