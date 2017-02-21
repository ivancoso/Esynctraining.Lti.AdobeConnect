using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        IAdobeConnectProxy GetProvider(ILmsLicense license, bool login = true);

        IAdobeConnectProxy GetProvider(string acDomain, UserCredentials credentials, bool login);

        ACDetailsDTO GetAccountDetails(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, ICache cache);

        IEnumerable<PrincipalReportDto> GetMeetingHostReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider);

        IEnumerable<TemplateDto> GetSharedMeetingTemplates(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, ICache cache);

        string LoginIntoAC(
            LmsCompany lmsCompany,
            LtiParamDTO param,
            Principal registeredUser,
            string password,
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider,
            bool updateAcUser = true);

    }

}