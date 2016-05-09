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
                throw new ArgumentNullException("config");
            if (recipients == null)
                throw new ArgumentNullException("recipients");

            return config.SystemEmails.GetByToken(recipients.FromToken).BuildMailAddress();
        }

        public static IEnumerable<MailAddress> GetTo(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (recipients == null)
                throw new ArgumentNullException("recipients");

            return recipients.ToTokens.Select(token => config.SystemEmails.GetByToken(token)).Select(x => x.BuildMailAddress());
        }

        public static IEnumerable<MailAddress> GetCc(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (recipients == null)
                throw new ArgumentNullException("recipients");

            return recipients.CcTokens.Select(token => config.SystemEmails.GetByToken(token)).Select(x => x.BuildMailAddress());
        }

        public static IEnumerable<MailAddress> GetBcc(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (recipients == null)
                throw new ArgumentNullException("recipients");

            return recipients.BccTokens.Select(token => config.SystemEmails.GetByToken(token)).Select(x => x.BuildMailAddress());
        }

    }

}
