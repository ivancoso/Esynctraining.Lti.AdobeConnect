namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        IAdobeConnectProxy GetProvider(IAdobeConnectAccess credentials, bool login);

        IAdobeConnectProxy GetProvider2(IAdobeConnectAccess2 credentials);

        IAdobeConnectProxy GetProvider2(IAdobeConnectAccess2 credentials, bool checkSessionIsAlive = true);

        ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider);

    }

}
