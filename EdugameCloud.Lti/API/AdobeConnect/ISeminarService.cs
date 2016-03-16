using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface ISeminarService : Esynctraining.AdobeConnect.ISeminarService
    {
        IEnumerable<SeminarLicenseDto> GetLicensesWithContent(IAdobeConnectProxy acProxy,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany);

        //SeminarSessionDto GetSeminarSessionInfo(IAdobeConnectProxy provider, string seminarSessionId);

        OperationResultWithData<SeminarSessionDto> SaveSeminarSession(SeminarSessionDto seminarSessionDto, IAdobeConnectProxy provider);

        //OperationResultWithData<SeminarSessionDto> SaveSeminarSession(SeminarSessionDto seminarSessionDto, IAdobeConnectProxy adminProvider, IAdobeConnectProxy userProvider, string acUsername);

    }

}
