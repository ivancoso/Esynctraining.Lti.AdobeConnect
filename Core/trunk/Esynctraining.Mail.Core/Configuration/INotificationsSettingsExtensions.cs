using System;
using System.Linq;
using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration
{
    public static class INotificationsSettingsExtensions
    {
        public static ISystemEmail GetFrom(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return config.SystemEmails.GetByToken(recipients.FromToken);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetFrom failed for {recipients.Token} email", ex);
            }
        }

        public static IEnumerable<ISystemEmail> GetTo(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return recipients.ToTokens.Select(token => config.SystemEmails.GetByToken(token));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetCc failed for {recipients.Token} email", ex);
            }
        }

        public static IEnumerable<ISystemEmail> GetCc(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return recipients.CcTokens.Select(token => config.SystemEmails.GetByToken(token));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetCc failed for {recipients.Token} email", ex);
            }
        }

        public static IEnumerable<ISystemEmail> GetBcc(this INotificationsSettings config, IEmailRecipientSettings recipients)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));

            try
            {
                return recipients.BccTokens.Select(token => config.SystemEmails.GetByToken(token));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"GetBcc failed for {recipients.Token} email", ex);
            }
        }

    }

}
