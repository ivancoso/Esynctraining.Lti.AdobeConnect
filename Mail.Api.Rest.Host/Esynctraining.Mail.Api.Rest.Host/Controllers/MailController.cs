using System;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Mail;
using Esynctraining.Mail.Configuration;
using Esynctraining.Mail.Configuration.Json;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChat.WebApi.Host.Controllers
{
    [Route("mail")]
    public class MailController : ControllerBase
    {
        private readonly INotificationsSettings _notificationsSettings;
        private readonly ISmtpClient _mail;
        private readonly ITemplateTransformer _templateTransformer;
        private readonly ILogger _logger;

        public MailController(INotificationsSettings notificationsSettings, ISmtpClient mail, ITemplateTransformer templateTransformer, ILogger logger)
        {
            _notificationsSettings = notificationsSettings ?? throw new ArgumentNullException(nameof(notificationsSettings));
            _mail = mail ?? throw new ArgumentNullException(nameof(mail));
            _templateTransformer = templateTransformer ?? throw new ArgumentNullException(nameof(templateTransformer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpPost]
        [Route("send")]
        public async Task<OperationResult> SendAsync([FromBody]EmailDto dto)
        {
            _logger.Info($"SendAsync TemplateToken = {dto.TemplateToken}; AdobeConnectUrl = {dto.AdobeConnectUrl}; RemoteIpAddress = {Request.HttpContext.Connection.RemoteIpAddress}");

            var recipients = _notificationsSettings.RecipientSettings.GetByToken(dto.TemplateToken);
            var from = _notificationsSettings.GetFrom(recipients);

            var to = dto.ToEmails.Select(x => new SystemEmail { Email = x.Email, Name = x.Name }).ToList();

            var cced = dto.CcEmails != null && dto.CcEmails.Any()
                           ? dto.CcEmails.Select(x => new SystemEmail { Email = x.Email, Name = x.Name }).ToList()
                           : null;

            var bcced = dto.BccEmails != null && dto.BccEmails.Any()
                            ? dto.BccEmails.Select(x => new SystemEmail { Email = x.Email, Name = x.Name }).ToList()
                            : null;

            var model = new EmailModel { MailBody = dto.BodyHtml, MailSubject = dto.Subject };
            var mailBody = await _templateTransformer.TransformAsync(dto.TemplateToken, model);
            
            var sendResult = await _mail.SendEmailAsync(from, to, dto.Subject, mailBody, cced, bcced);

            return new OperationResult { IsSuccess = sendResult };
        }

    }

}
