using System.Configuration;

namespace Esynctraining.AdobeConnect.Api.Configuration
{
    public class CredentialsElement : ConfigurationElement, ICredentials
    {
        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
        }

    }

}
