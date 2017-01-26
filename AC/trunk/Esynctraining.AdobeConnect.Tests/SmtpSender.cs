using System;
using System.Net.Mail;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Tests
{
    public class SmtpSender
    {
        private SmtpSettings _smtpSettings;
        private ILogger _logger;

        public SmtpSender(SmtpSettings settings, ILogger logger)
        {
            _logger = logger;
            _smtpSettings = settings;
        }

        public void SendEmail(string subj, string text)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);

                mail.From = new MailAddress(_smtpSettings.FromEmail);
                mail.To.Add(_smtpSettings.ToEmail);
                mail.Subject = subj;
                mail.Body = text;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                _logger.Error("Sending email error", e);
            }
        }
    }


    public class SmtpSettings
    {
        public SmtpSettings(string host, string username, string password, string fromEmail, string toEmail,
            int port = 25)
        {
            Host = host;
            Username = username;
            Password = password;
            FromEmail = fromEmail;
            ToEmail = toEmail;
            Port = port;
        }

        public string FromEmail { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ToEmail { get; set; }
    }

}