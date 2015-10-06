using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        IAdobeConnectProxy GetProvider(LmsCompany license, bool login = true);

        IAdobeConnectProxy GetProvider(LmsCompany license, UserCredentials credentials, bool login);

        ACPasswordPoliciesDTO GetPasswordPolicies(IAdobeConnectProxy provider);

        IEnumerable<PrincipalReportDto> GetMeetingHostReport(IAdobeConnectProxy provider);

        IEnumerable<TemplateDTO> GetTemplates(IAdobeConnectProxy provider, string templateFolder);

        string LoginIntoAC(
            LmsCompany lmsCompany,
            LtiParamDTO param,
            Principal registeredUser,
            string password,
            IAdobeConnectProxy provider,
            bool updateAcUser = true);
    }
}