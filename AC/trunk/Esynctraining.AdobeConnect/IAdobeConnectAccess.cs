using System;

namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectAccess
    {
        string Domain { get; set; }

        string Login { get; set; }

        string Password { get; set; }
    }

    public interface IAdobeConnectAccess2
    {
        string Domain { get; set; }

        string SessionToken { get; set; }
    }

    public class AdobeConnectAccess : IAdobeConnectAccess
    {
        public string Domain { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }


        public AdobeConnectAccess(string domain, string login, string password)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Domain can't be empty", "domain");
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Login can't be empty", "login");
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Password can't be empty", "password");

            Domain = domain;
            Login = login;
            Password = password;
        }

    }

    public class AdobeConnectAccess2 : IAdobeConnectAccess2
    {
        public string Domain { get; set; }

        public string SessionToken { get; set; }


        public AdobeConnectAccess2(string domain, string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Domain can't be empty", "domain");
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("SessionToken can't be empty", "sessionToken");

            Domain = domain;
            SessionToken = sessionToken;
        }
        
    }

}
