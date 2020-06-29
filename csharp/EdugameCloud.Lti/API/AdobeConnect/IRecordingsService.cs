using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IRecordingsService
    {
        PagedResult<IRecordingDto> GetRecordings(ILmsLicense lmsCompany, 
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider,
            string courseId, 
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

        Task<(string url, string breezeSession)> JoinRecording(ILmsLicense lmsCompany, LtiParamDTO param, string recordingUrl,
            IAdobeConnectProxy adobeConnectProvider, string mode = null);

        OperationResult EditRecording(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            string courseId,
            string recordingId,
            int id,
            string name,
            string summary);

        OperationResult RemoveRecording(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            string courseId,
            string recordingId,
            int id);

        string GetPasscode(ILmsLicense lmsCompany, IAdobeConnectProxy provider, string scoId);

    }

}