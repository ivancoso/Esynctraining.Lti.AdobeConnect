namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectAccess
    {
        string Domain { get; set; }

        string Login { get; set; }

        string Password { get; set; }
    }

    public class AdobeConnectAccess : IAdobeConnectAccess
    {
        public string Domain { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }


        public AdobeConnectAccess()
        {
            // HACK: hardcoded
            Domain = "http://connectdev.esynctraining.com";
            Login = "sergeyi@esynctraining.com";
            Password = "e$ync123";
        }

    }

}
