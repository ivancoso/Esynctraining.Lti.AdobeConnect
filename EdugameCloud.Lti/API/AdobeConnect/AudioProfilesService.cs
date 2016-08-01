using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class AudioProfilesService : IAudioProfilesService
    {
        private readonly LmsCourseMeetingModel meetingModel;
        private readonly ILogger logger;


        public AudioProfilesService(LmsCourseMeetingModel meetingModel, ILogger logger)
        {
            if (meetingModel == null)
                throw new ArgumentNullException(nameof(meetingModel));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.meetingModel = meetingModel;
            this.logger = logger;
        }


        public IEnumerable<TelephonyProfile> GetAudioProfiles(IAdobeConnectProxy provider, LmsCompany lmsCompany, string principalId)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var usedAudioProfiles = lmsCompany.GetSetting<bool>(LmsCompanySettingNames.AudioProfileUnique) 
                ? meetingModel.GetByCompanyWithAudioProfiles(lmsCompany).ToList().Select(x => x.AudioProfileId).ToList() 
                : new List<string>();

            var telephonyPrfilesListResult = provider.TelephonyProfileList(principalId ?? provider.PrincipalId);
            var profiles = telephonyPrfilesListResult.Values.Where(x => !usedAudioProfiles.Contains(x.ProfileId)).ToList();
            return profiles;
        }

        public OperationResult AddAudioProfileToMeeting(string meetingScoId, string audioProfileId, 
            IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));
            if (string.IsNullOrWhiteSpace(audioProfileId))
                throw new ArgumentException("Non-empty value expected", nameof(audioProfileId));

            //todo: telephony-profile-info to AC provider
            var telephonyProfileListResult = provider.TelephonyProfileInfo(audioProfileId);

            if (telephonyProfileListResult.Success && telephonyProfileListResult.TelephonyProfile != null)
            {
                //if(telephonyPrfilesListResult.Values.Any(x => x.ProfileId.Equals(audioProfileId)))
                {
                    provider.UpdateAclField(meetingScoId, AclFieldId.telephony_profile, audioProfileId);
                    return OperationResult.Success();
                }

                //logger.ErrorFormat($"Couldn't get audio profile. PrincipalId={principalId ?? provider.PrincipalId}, profileId={audioProfileId}.");
                //return OperationResult.Error("Couldn't get audio profile. Please refresh page and try again.");
            }

            logger.ErrorFormat($"Error occured when tried to get audio profiles. ProfileId={audioProfileId}.");
            return OperationResult.Error("Unexpected error. Please refresh page and try again.");
        }

        //public OperationResult UpdateAudioProfileId(LmsCourseMeeting meeting, IAdobeConnectProxy provider, string audioProfileId)
        //{
        //    if (meeting == null)
        //        throw new ArgumentNullException(nameof(meeting));
        //    if (provider == null)
        //        throw new ArgumentNullException(nameof(provider));

        //    var opResult = AddAudioProfileToMeeting(meeting.ScoId, audioProfileId, provider);
        //    if (!opResult.IsSuccess)
        //    {
        //        return opResult;
        //    }

        //    meeting.AudioProfileId = audioProfileId;

        //    meetingModel.RegisterSave(meeting, flush: true);

        //    return OperationResultWithData<LmsCourseMeeting>.Success("Meeting audio profile updated", meeting);
        //}

    }

}