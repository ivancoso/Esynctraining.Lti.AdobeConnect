using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAudioProfilesService
    {
        IEnumerable<TelephonyProfile> GetAudioProfiles(IAdobeConnectProxy provider, LmsCompany lmsCompany, string principalId);

        OperationResult AddAudioProfileToMeeting(string meetingScoId, string audioProfileId,
            IAdobeConnectProxy provider, string principalId);

        OperationResult UpdateAudioProfileId(LmsCourseMeeting meeting, IAdobeConnectProxy provider,
            string audioProfileId, string principalId);
    }
}