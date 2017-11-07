using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Email;

namespace Serilog
{
    public static class LoggerConfigurationEmailExtensions
    {
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string mailServer,
            string mailUserName,
            string mailPassword,
            bool enableSsl = false,
            int port = 25,
            string mailSubject = EmailConnectionInfo.DefaultSubject,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmail == null) throw new ArgumentNullException("toEmail");

            var connectionInfo = new EmailConnectionInfo
            {
                FromEmail = fromEmail,
                ToEmail = toEmail,
                MailServer = mailServer,
                Port = port,
                NetworkCredentials = new NetworkCredential(mailUserName, mailPassword),
                EmailSubject = mailSubject,
                EnableSsl = enableSsl,
                ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    return true;
                }
            };

            return loggerConfiguration.Email(connectionInfo, restrictedToMinimumLevel: restrictedToMinimumLevel);
        }
    }
}
