using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Api.AudioProfiles
{
    public class AudioProfilesService : IAudioProfilesService
    {
        private readonly ILogger logger;


        public AudioProfilesService(ILogger logger)
        {
            this.logger = logger;
        }


        public IEnumerable<TelephonyProfile> GetAudioProfiles(IAdobeConnectProxy provider, string principalId)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            var telephonyPrfilesListResult = provider.TelephonyProfileList(principalId);
            var profiles = telephonyPrfilesListResult.Values.ToList();
            return profiles;
        }

        public TelephonyProfile GetAudioProfileInfo(IAdobeConnectProxy provider, string profileId)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(profileId))
                throw new ArgumentException("Non-empty value expected", nameof(profileId));

            var profile = provider.TelephonyProfileInfo(profileId);
            return profile.Success ? profile.TelephonyProfile : null;
        }

        public OperationResult AddAudioProfileToMeeting(IAdobeConnectProxy provider, string meetingScoId, string audioProfileId)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));
            if (string.IsNullOrWhiteSpace(audioProfileId))
                throw new ArgumentException("Non-empty value expected", nameof(audioProfileId));

            //todo: telephony-profile-info to AC provider
            var telephonyProfileInfoResult = provider.TelephonyProfileInfo(audioProfileId);

            if (telephonyProfileInfoResult.Success && telephonyProfileInfoResult.TelephonyProfile != null)
            {
                    provider.UpdateAclField(meetingScoId, AclFieldId.telephony_profile, audioProfileId);
                    return OperationResult.Success();
            }

            logger.ErrorFormat($"Error occured when tried to AddAudioProfileToMeeting. ProfileId={audioProfileId}.");
            return OperationResult.Error("Couldn't get audio profile.");
        }

    }

}