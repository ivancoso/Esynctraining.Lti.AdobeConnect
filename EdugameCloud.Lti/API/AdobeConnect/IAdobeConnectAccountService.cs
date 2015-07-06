using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.AC.Provider;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        ACPasswordPoliciesDTO GetPasswordPolicies(IAdobeConnectProxy provider);

        IEnumerable<PrincipalReportDto> GetMeetingHostReport(IAdobeConnectProxy provider);

    }

}