using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface ISeminarService : Esynctraining.AdobeConnect.ISeminarService
    {
        IEnumerable<SeminarLicenseDto> GetLicensesWithContent(IAdobeConnectProxy acProxy,
            IEnumerable<LmsCourseMeeting> seminarRecords,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany,
            TimeZoneInfo timeZone);

        OperationResultWithData<SeminarSessionDto> SaveSeminarSession(SeminarSessionDto seminarSessionDto, 
            IAdobeConnectProxy provider,
            TimeZoneInfo timeZone);

    }

}
