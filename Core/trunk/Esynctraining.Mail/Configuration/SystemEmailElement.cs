using System.Configuration;
using System.Net.Mail;

namespace Esynctraining.Mail.Configuration
{
    internal sealed class SystemEmailElement : ConfigurationElement, ISystemEmail
    {
        [ConfigurationProperty("token", IsRequired = true, IsKey = true)]
        public string Token
        {
            get { return (string)this["token"]; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("email", IsRequired = true)]
        //[RegexStringValidator(CommonExpressions.Email)]
        public string Email
        {
            get { return (string)this["email"]; }
        }


        public MailAddress BuildMailAddress()
        {
            return new MailAddress(Email, Name);
        }

    }

}
