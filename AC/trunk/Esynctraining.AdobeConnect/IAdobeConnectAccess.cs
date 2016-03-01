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
            Domain = domain;
            Login = login;
            Password = password;
        }

    }

    public class AdobeConnectAccess2 : IAdobeConnectAccess2
    {
        public string Domain { get; set; }

        public string SessionToken { get; set; }

    }

}
