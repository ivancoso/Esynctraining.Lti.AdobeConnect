using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IRecordingsService
    {
        PagedResult<IRecordingDto> GetRecordings(ILmsLicense lmsCompany, 
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider,
            int courseId, 
            int id,
            Func<IRoomTypeFactory> getRoomTypeFactory,
            string sortBy,
            string sortOder,
            string search,
            long? dateFrom,
            long? dateTo,
            Func<IEnumerable<IRecordingDto>, IEnumerable<IRecordingDto>> applyAdditionalFilter,
            int skip,
            int take);

        string UpdateRecording(ILmsLicense lmsCompany, IAdobeConnectProxy provider, string id, bool isPublic,
            string password);

        string JoinRecording(ILmsLicense lmsCompany, LtiParamDTO param, string recordingUrl,
            ref string breezeSession, IAdobeConnectProxy adobeConnectProvider, string mode = null);

        OperationResult EditRecording(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id,
            string name,
            string summary);

        OperationResult RemoveRecording(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id);

        string GetPasscode(ILmsLicense lmsCompany, string scoId);

    }

}