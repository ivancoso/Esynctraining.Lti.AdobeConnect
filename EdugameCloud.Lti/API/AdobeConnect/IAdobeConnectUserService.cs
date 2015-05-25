using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectUserService
    {
        Principal GetOrCreatePrincipal(
            AdobeConnectProvider provider,
            string login,
            string email,
            string firstName,
            string lastName,
            LmsCompany lmsCompany);

        Principal GetOrCreatePrincipal2(
            AdobeConnectProvider provider,
            string login,
            string email,
            string firstName,
            string lastName,
            LmsCompany lmsCompany,
            IEnumerable<Principal> principalCache);

        Principal GetPrincipalByLoginOrEmail(
            AdobeConnectProvider provider,
            string login,
            string email,
            bool searchByEmailFirst);
    }
}