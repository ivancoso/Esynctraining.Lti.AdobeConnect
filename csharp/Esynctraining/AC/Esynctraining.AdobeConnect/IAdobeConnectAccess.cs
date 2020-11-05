using System;

namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectAccess
    {
        Uri Domain { get; }

        string Login { get; }

        string Password { get; }
    }

    public interface IAdobeConnectAccess2
    {
        Uri Domain { get; }

        string SessionToken { get; }
    }

    public class AdobeConnectAccess : IAdobeConnectAccess
    {
        public Uri Domain { get; }

        public string Login { get; }

        public string Password { get; }


        public AdobeConnectAccess(Uri domain, string login, string password)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));
            if (!domain.IsAbsoluteUri)
                throw new ArgumentException("Absolute Uri expected", nameof(domain));
            if (!domain.Scheme.Equals("HTTPS", StringComparison.OrdinalIgnoreCase) && !domain.Scheme.Equals("HTTP", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"HTTP and HTTPS only", nameof(domain));
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login can't be empty", nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password can't be empty", nameof(password));

            Domain = domain;
            Login = login;
            Password = password;
        }

    }

    public class AdobeConnectAccess2 : IAdobeConnectAccess2
    {
        public Uri Domain { get; }

        public string SessionToken { get; }


        public AdobeConnectAccess2(Uri domain, string sessionToken)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));
            if (!domain.IsAbsoluteUri)
                throw new ArgumentException("Absolute Uri expected", nameof(domain));
            if (!domain.Scheme.Equals("HTTPS", StringComparison.OrdinalIgnoreCase) && !domain.Scheme.Equals("HTTP", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"HTTP and HTTPS only", nameof(domain));
            if (string.IsNullOrWhiteSpace(sessionToken))
                throw new ArgumentException("SessionToken can't be empty", nameof(sessionToken));

            Domain = domain;
            SessionToken = sessionToken;
        }
        
    }

}
