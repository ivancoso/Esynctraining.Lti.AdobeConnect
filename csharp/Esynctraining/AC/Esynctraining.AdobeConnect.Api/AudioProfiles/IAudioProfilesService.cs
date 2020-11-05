using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Domain;

namespace Esynctraining.AdobeConnect.Api.AudioProfiles
{
    public interface IAudioProfilesService
    {
        IEnumerable<TelephonyProfile> GetAudioProfiles(IAdobeConnectProxy provider, string principalId);

        TelephonyProfile GetAudioProfileInfo(IAdobeConnectProxy provider, string profileId);

        OperationResult AddAudioProfileToMeeting(IAdobeConnectProxy provider, string meetingScoId, string audioProfileId);

    }

}