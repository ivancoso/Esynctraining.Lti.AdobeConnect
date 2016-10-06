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
        IEnumerable<IRecordingDto> GetRecordings(LmsCompany lmsCompany, 
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider,
            int courseId, 
            int id,
            Func<IRoomTypeFactory> getRoomTypeFactory);

        string UpdateRecording(LmsCompany lmsCompany, IAdobeConnectProxy provider, string id, bool isPublic,
            string password);

        string JoinRecording(LmsCompany lmsCompany, LtiParamDTO param, string recordingUrl,
            ref string breezeSession, IAdobeConnectProxy adobeConnectProvider, string mode = null);

        OperationResult EditRecording(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id,
            string name,
            string summary);

        OperationResult RemoveRecording(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            int courseId,
            string recordingId,
            int id);

    }

}