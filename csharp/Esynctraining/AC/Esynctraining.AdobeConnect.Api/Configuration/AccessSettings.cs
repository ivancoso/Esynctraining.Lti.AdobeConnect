using System;

namespace Esynctraining.AdobeConnect.Api.Configuration
{
    public class AccessSettingsDto
    {
        public string Domain { get; set; }

        public Credentials AdminCredentials { get; set; }

        public class Credentials
        {
            public string UserName { get; set; }

            public string Password { get; set; }

        }

        public AccessSettingsDto()
        {
            AdminCredentials = new Credentials();
        }
    }

    public class AccessSettings : IAccessSettings
    {
        private string _domain;


        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                var str = value;

                Uri url;
                if (!Uri.TryCreate(str, UriKind.Absolute, out url))
                {
                    throw new ArgumentException($"Domain value '{str}' is not valid (Absolute URL expected)");
                }

                if (!url.Scheme.Equals("HTTPS", StringComparison.OrdinalIgnoreCase) &&
                    !url.Scheme.Equals("HTTP", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Domain value '{str}' is not valid (HTTP and HTTPS only)");

                _domain = url.ToString().TrimEnd('/');
            }
        }
        
        public Credentials AdminCredentials { get; set; }

        ICredentials IAccessSettings.AdminCredentials
        {
            get { return AdminCredentials; }
        }


        public AccessSettings() { }

        public AccessSettings(AccessSettingsDto source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.Domain = source.Domain;
            this.AdminCredentials = new Credentials
            {
                UserName = source.AdminCredentials.UserName,
                Password = source.AdminCredentials.Password,
            };
        }


        public AdobeConnectAccess Build()
        {
            return new AdobeConnectAccess(new Uri(Domain), AdminCredentials.UserName, AdminCredentials.Password);
        }

        public class Credentials : ICredentials
        {
            public string UserName { get; set; }

            public string Password { get; set; }

        }

    }

}
