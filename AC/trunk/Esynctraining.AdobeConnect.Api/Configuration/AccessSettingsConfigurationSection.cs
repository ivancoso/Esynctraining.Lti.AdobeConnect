using System;
using System.Configuration;

namespace Esynctraining.AdobeConnect.Api.Configuration
{
    public class AccessSettingsConfigurationSection : ConfigurationSection, IAccessSettings
    {
        public string ApiUrl
        {
            get
            {
                return new Uri(new Uri(Domain), "api/xml").ToString();
            }
        }

        [ConfigurationProperty("domain", IsRequired = true, Options = ConfigurationPropertyOptions.IsRequired)]
        public string Domain
        {
            get
            {
                var str = this["domain"] as string;

                Uri url;
                if (!Uri.TryCreate(str, UriKind.Absolute, out url))
                {
                    throw new ConfigurationErrorsException($"apiUrl value '{str}' is not valid (Absolute URL expected)");
                }
                return url.ToString().TrimEnd('/');
            }
        }


        ICredentials IAccessSettings.AdminCredentials
        {
            get { return Admin; }
        }

        [ConfigurationProperty("admin", IsRequired = false)] // required??
        private CredentialsElement Admin
        {
            get
            {
                return (CredentialsElement)this["admin"];
            }
        }

        public AdobeConnectAccess Build()
        {
            return new AdobeConnectAccess(Domain, Admin.UserName, Admin.Password);
        }

    }

}
