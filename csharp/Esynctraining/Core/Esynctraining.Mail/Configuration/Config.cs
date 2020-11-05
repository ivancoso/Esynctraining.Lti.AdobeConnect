using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;

namespace Esynctraining.Mail.Configuration
{
    public static class Config
    {
        // TODO: implement generic method with 'name' parameter
        public static INotificationsSettings GetConfig()
        {
            var result = (INotificationsSettings)ConfigurationManager.GetSection("mailerSettings");

            if (result == null)
                throw new ConfigurationErrorsException("Configuration section 'mailerSettings' was not found.");

            return result;
        }

        public static MailAddress GetFrom(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return config.SystemEmails.GetByToken(recipients.FromToken).BuildMailAddress();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetFrom failed for {recipients.Token} email", ex);
            }
        }

        public static IEnumerable<MailAddress> GetTo(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return recipients.ToTokens.Select(token => config.SystemEmails.GetByToken(token)).Select(x => x.BuildMailAddress());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetTo failed for {recipients.Token} email", ex);
            }
        }

        public static IEnumerable<MailAddress> GetCc(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return recipients.CcTokens.Select(token => config.SystemEmails.GetByToken(token)).Select(x => x.BuildMailAddress());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetCc failed for {recipients.Token} email", ex);
            }
        }

        public static IEnumerable<MailAddress> GetBcc(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return recipients.BccTokens.Select(token => config.SystemEmails.GetByToken(token)).Select(x => x.BuildMailAddress());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetBcc failed for {recipients.Token} email", ex);
            }
        }

    }

}
