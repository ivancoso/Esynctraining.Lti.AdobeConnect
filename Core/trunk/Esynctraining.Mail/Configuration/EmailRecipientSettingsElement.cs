using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Esynctraining.Mail.Configuration
{
    internal sealed class EmailRecipientSettingsElement : ConfigurationElement, IEmailRecipientSettings
    {
        [ConfigurationProperty("token", IsRequired = true, IsKey = true)]
        public string Token
        {
            get { return (string)this["token"]; }
        }

        [ConfigurationProperty("to", IsRequired = false)]
        private string To
        {
            get { return (string)this["to"]; }
        }

        [ConfigurationProperty("cc", IsRequired = false)]
        private string Cc
        {
            get { return (string)this["cc"]; }
        }

        [ConfigurationProperty("bcc", IsRequired = false)]
        private string Bcc
        {
            get { return (string)this["bcc"]; }
        }

        public IEnumerable<string> ToTokens
        {
            get
            {
                return ParseTokens(To);
            }
        }

        public IEnumerable<string> CcTokens
        {
            get
            {
                return ParseTokens(Cc);
            }
        }

        public IEnumerable<string> BccTokens
        {
            get
            {
                return ParseTokens(Bcc);
            }
        }


        private static IEnumerable<string> ParseTokens(string setting)
        {
            if (string.IsNullOrWhiteSpace(setting))
                return Enumerable.Empty<string>();

            return setting.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

}
