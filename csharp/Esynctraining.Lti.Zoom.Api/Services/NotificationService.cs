using System;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Zoom.Api.Dto.Email;
using Esynctraining.Lti.Zoom.Common.Dto.OfficeHours;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Mail;
using Esynctraining.Mail.Configuration;
using Esynctraining.Mail.Configuration.Json;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class NotificationService : INotificationService
    {
        //private readonly INotificationsSettings _notificationsSettings;
        private readonly ISmtpClient _smtpClientEngine;
        private readonly ITemplateTransformer _templateTransformer;
        private readonly ILogger _logger;
        //private readonly IStringLocalizer<NotificationService> _localizer;

        public NotificationService(
            
            ISmtpClient smtpClientEngine,
            ITemplateTransformer templateTransformer,
            ILogger logger)
        {
            
            _smtpClientEngine = smtpClientEngine ?? throw new ArgumentNullException(nameof(smtpClientEngine));
            _templateTransformer = templateTransformer ?? throw new ArgumentNullException(nameof(templateTransformer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
        }

        public async Task<bool> SendOHBookSlotEmail(SlotDto dto, string meetingName, string email, string name)
        {
            var emailDto = new OfficeHoursBookSlotEmailModel
            {
                UserName = dto.UserName,
                Date = dto.Start.ToString(),
                MeetingName = meetingName,
                Subject = dto.Subject,
                Questions = dto.Questions
            };

            var from = new SystemEmail { Name = "no-reply", Email = "no-reply@esynctraining.com" };
            ISystemEmail to = new SystemEmail { Name = name, Email = email };

            var subject = $"Confirmation: '{dto.UserName}' has booked the Session.";

            return await _smtpClientEngine.SendEmailAsync<OfficeHoursBookSlotEmailModel>(from, new[] { to }, subject, emailDto);
        }

        public async Task<bool> SendOHCancellationEmail(DateTime date, string meetingName, string message, string email)
        {
            return true;
            var emailDto = new OfficeHoursCancellationEmailModel
            {
                Date = date.ToString(),
                MeetingName = meetingName,
                Message = message
            };

            var from = new SystemEmail { Name = "no-reply", Email = "no-reply@esynctraining.com" };
            ISystemEmail to = new SystemEmail { Name = email, Email = email };

            var subject = $"Office Hours meeting '{meetingName}' cancellation.";

            return await _smtpClientEngine.SendEmailAsync<OfficeHoursCancellationEmailModel>(from, new []{to}, subject, emailDto);
        }

        public async Task<bool> SendOHRescheduleEmail(DateTime date, SlotDto dto, string meetingName, string message)
        {
            return true;
            var emailDto = new OfficeHoursRescheduleEmailModel
            {
                Date = date.ToString(),
                MeetingName = meetingName,
                Message = message,

                NewDate = dto.Start.ToString(),
                Subject = dto.Subject,
                Questions = dto.Questions

            };

            var from = new SystemEmail { Name = "no-reply", Email = "no-reply@esynctraining.com" };
            ISystemEmail to = new SystemEmail { Name = dto.UserName, Email = dto.UserName };

            var subject = $"Office Hours session for '{meetingName}' cancellation.";

            return await _smtpClientEngine.SendEmailAsync<OfficeHoursRescheduleEmailModel>(from, new[] { to }, subject, emailDto);
        }
    }
}