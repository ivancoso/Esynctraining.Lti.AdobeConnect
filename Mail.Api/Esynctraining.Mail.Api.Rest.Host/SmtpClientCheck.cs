using System;
using System.Threading;
using System.Threading.Tasks;
using Esynctraining.Mail.Configuration.Json;
using Microsoft.Extensions.HealthChecks;

namespace Esynctraining.Mail.Api.Rest.Host
{
    internal sealed class SmtpClientCheck : IHealthCheck
    {
        private readonly ISmtpClient _mail;


        public SmtpClientCheck(ISmtpClient mail)
        {
            _mail = mail ?? throw new ArgumentNullException(nameof(mail));
        }


        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var sendResult = await _mail.SendEmailAsync(
                    new SystemEmail { Email = "system@esynctraining.com" }, 
                    new[] { new SystemEmail { Email = "system@esynctraining.com" } }, 
                    "Subject", 
                    "Test Body", 
                    null, 
                    null);

                return sendResult
                    ? HealthCheckResult.Healthy("SMTP Check: Healthy")
                    : HealthCheckResult.Unhealthy("SMTP Check: Unhealthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"SMTP Check: Exception during check: {ex.Message} {ex.GetType().FullName}");
            }
        }

    }

}
