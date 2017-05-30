using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration.Json
{
    public sealed class EmailRecipientSettings : IEmailRecipientSettings
    {
        //[ConfigurationProperty("token", IsRequired = true, IsKey = true)]
        public string Token { get; set; }

        public string FromToken { get; set; }

        public List<string> ToTokens { get; set; }

        public List<string> CcTokens { get; set; }

        public List<string> BccTokens { get; set; }


        public EmailRecipientSettings()
        {
            ToTokens = new List<string>();
            CcTokens = new List<string>();
            BccTokens = new List<string>();
        }
    }

}
