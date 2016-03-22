using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IRecordingsService
    {
        IEnumerable<RecordingDTO> GetRecordings(LmsCompany lmsCompany, 
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider,
            int courseId, 
            int id,
            Func<IRoomTypeFactory> getRoomTypeFactory);

        string UpdateRecording(LmsCompany lmsCompany, IAdobeConnectProxy provider, string id, bool isPublic,
            string password);

        string JoinRecording(LmsCompany lmsCompany, LtiParamDTO param, string recordingUrl,
            ref string breezeSession, string mode = null, IAdobeConnectProxy adobeConnectProvider = null);

        OperationResult RemoveRecording(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id);

    }

}