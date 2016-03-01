namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        IAdobeConnectProxy GetProvider(IAdobeConnectAccess credentials, bool login);

        IAdobeConnectProxy GetProvider2(IAdobeConnectAccess2 credentials);

        ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider);

    }

}
