namespace Esynctraining.AdobeConnect.Api.Configuration
{
    public interface IAccessSettings
    {
        string Domain { get; }

        ICredentials AdminCredentials { get; }


        AdobeConnectAccess Build();
    }

    public interface ICredentials
    {
        string UserName { get; }

        string Password { get; }

    }

}
