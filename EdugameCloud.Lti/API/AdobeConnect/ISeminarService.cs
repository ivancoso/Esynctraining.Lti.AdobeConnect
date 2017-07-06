using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface ISeminarService : Esynctraining.AdobeConnect.ISeminarService
    {
        IEnumerable<SeminarLicenseDto> GetLicensesWithContent(IAdobeConnectProxy acProxy,
            IEnumerable<LmsCourseMeeting> seminarRecords,
            LmsUser lmsUser,
            LtiParamDTO param,
            LmsCompany lmsCompany//,
            //TimeZoneInfo timeZone
            );

        OperationResultWithData<Esynctraining.AdobeConnect.Api.Seminar.Dto.SeminarSessionDto> SaveSeminarSession(SeminarSessionInputDto seminarSessionDto,
            string seminarScoId,
            IAdobeConnectProxy provider//,
            //TimeZoneInfo timeZone
            );

        //bool CheckEditPermissions(string licenseId, LtiParamDTO param, IAdobeConnectProxy acProxy, LmsUser lmsUser);

    }

}
