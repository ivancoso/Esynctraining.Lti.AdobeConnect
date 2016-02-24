namespace Esynctraining.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        IAdobeConnectProxy GetProvider(IAdobeConnectAccess credentials, bool login);

        ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider);

    }

}
