using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class AudioProfilesService : IAudioProfilesService
    {
        private LmsCourseMeetingModel meetingModel;
        private ILogger logger;

        public AudioProfilesService(LmsCourseMeetingModel meetingModel, ILogger logger)
        {
            this.meetingModel = meetingModel;
            this.logger = logger;
        }

        public IEnumerable<TelephonyProfile> GetAudioProfiles(IAdobeConnectProxy provider, LmsCompany lmsCompany, string principalId)
        {
            var usedAudioProfiles = lmsCompany.GetSetting<bool>(LmsCompanySettingNames.AudioProfileUnique) 
                ? meetingModel.GetByCompanyWithAudioProfiles(lmsCompany).ToList().Select(x => x.AudioProfileId).ToList() 
                : new List<string>();

            var telephonyPrfilesListResult = provider.TelephonyProfileList(principalId ?? provider.PrincipalId);
            var profiles = telephonyPrfilesListResult.Values.Where(x => !usedAudioProfiles.Contains(x.ProfileId)).ToList();
            return profiles;
        }

        public OperationResult AddAudioProfileToMeeting(string meetingScoId, string audioProfileId, 
            IAdobeConnectProxy provider, string principalId)
        {
            //todo: telephony-profile-info to AC provider
            var telephonyPrfilesListResult = provider.TelephonyProfileList(principalId ?? provider.PrincipalId);

            if (telephonyPrfilesListResult.Success)
            {
                if(telephonyPrfilesListResult.Values.Any(x => x.ProfileId.Equals(audioProfileId)))
                {
                    provider.UpdateAclField(meetingScoId, AclFieldId.telephony_profile, audioProfileId);
                    return OperationResult.Success();
                }

                logger.ErrorFormat($"Couldn't get audio profile. PrincipalId={principalId??provider.PrincipalId}, profileId={audioProfileId}.");
                return OperationResult.Error("Couldn't get audio profile. Please refresh page and try again.");
            }

            logger.ErrorFormat($"Error occured when tried to get audio profiles. PrincipalId={principalId ?? provider.PrincipalId}, profileId={audioProfileId}.");
            return OperationResult.Error("Unexpected error. Please refresh page and try again.");
        }

        public OperationResult UpdateAudioProfileId(LmsCourseMeeting meeting, IAdobeConnectProxy provider,
            string audioProfileId, string principalId)
        {
            if (meeting == null)
                throw new ArgumentNullException("meeting");
            if (provider == null)
                throw new ArgumentNullException("provider");

            var opResult = AddAudioProfileToMeeting(meeting.ScoId, audioProfileId, provider, principalId);
            if (!opResult.isSuccess)
            {
                return opResult;
            }

            meeting.AudioProfileId = audioProfileId;

            meetingModel.RegisterSave(meeting);
            meetingModel.Flush();

            return OperationResult.Success("Meeting audio profile updated", meeting);
        }
    }
}