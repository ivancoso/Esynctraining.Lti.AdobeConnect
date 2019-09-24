using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectUserService
    {
        Principal GetOrCreatePrincipal(
            IAdobeConnectProxy provider,
            string login,
            string email,
            string firstName,
            string lastName,
            ILmsLicense lmsCompany);

        Principal GetOrCreatePrincipal2(
            IAdobeConnectProxy provider,
            string login,
            string email,
            string firstName,
            string lastName,
            ILmsLicense lmsCompany,
            IEnumerable<Principal> principalCache);

        Principal GetPrincipalByLoginOrEmail(
            IAdobeConnectProxy provider,
            string login,
            string email,
            bool searchByEmailFirst);

        Principal UpdatePrincipalName(
            IAdobeConnectProxy provider,
            string principalId,
            string firstName,
            string lastName);
    }
}