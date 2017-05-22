using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration.Json
{
    public sealed class EmailRecipientSettings : IEmailRecipientSettings
    {
        //[ConfigurationProperty("token", IsRequired = true, IsKey = true)]
        public string Token { get; set; }

        public string FromToken { get; set; }

        public IEnumerable<string> ToTokens { get; set; }

        public IEnumerable<string> CcTokens { get; set; }

        public IEnumerable<string> BccTokens { get; set; }


        public EmailRecipientSettings()
        {
            ToTokens = new string[0];
            CcTokens = new string[0];
            BccTokens = new string[0];
        }
    }

}
