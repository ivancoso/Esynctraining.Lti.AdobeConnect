using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        IAdobeConnectProxy GetProvider(ILmsLicense license, bool login = true);

        IAdobeConnectProxy GetProvider(string acDomain, UserCredentials credentials, bool login);

        ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider, ICache cache);

        IEnumerable<PrincipalReportDto> GetMeetingHostReport(IAdobeConnectProxy provider);

        string LoginIntoAC(
            ILmsLicense lmsCompany,
            LtiParamDTO param,
            Principal registeredUser,
            string password,
            IAdobeConnectProxy provider,
            bool updateAcUser = true);
    }

}