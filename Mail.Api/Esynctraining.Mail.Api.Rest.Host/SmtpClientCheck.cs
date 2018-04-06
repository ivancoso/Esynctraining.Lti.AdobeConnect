using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Esynctraining.Mail.Configuration;
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

        public ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var sendResult = _mail.SendEmailAsync(
                    new SystemEmail { Email = "system@esynctraining.com" }, 
                    new[] { new SystemEmail { Email = "system@esynctraining.com" } }, 
                    "Subject", 
                    "Test Body", 
                    null, 
                    null).Result;

                if (sendResult)
                    return new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("SMTP Check: Healthy"));
                else
                    return new ValueTask<IHealthCheckResult>(HealthCheckResult.Unhealthy("SMTP Check: Unhealthy"));
            }
            catch (Exception ex)
            {
                return new ValueTask<IHealthCheckResult>(HealthCheckResult.Unhealthy($"SMTP Check: Exception during check: {ex.Message} {ex.GetType().FullName}"));
            }
        }
    }
}
