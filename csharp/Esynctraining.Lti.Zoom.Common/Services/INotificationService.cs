using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto.OfficeHours;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public interface INotificationService
    {
        Task<bool> SendOHBookSlotEmail(SlotDto dto, string meetingName, string email, string name);
        Task<bool> SendOHCancellationEmail(DateTime date, string meetingName, string message, string email);
        Task<bool> SendOHRescheduleEmail(DateTime date, SlotDto dto, string meetingName, string message);
    }
}