using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class AudioProfilesService : IAudioProfilesService
    {
        private readonly LmsCourseMeetingModel _meetingModel;
        private readonly ILogger _logger;
        private readonly Esynctraining.AdobeConnect.Api.AudioProfiles.AudioProfilesService _innerService;


        public AudioProfilesService(LmsCourseMeetingModel meetingModel, ILogger logger)
        {
            _meetingModel = meetingModel ?? throw new ArgumentNullException(nameof(meetingModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _innerService = new Esynctraining.AdobeConnect.Api.AudioProfiles.AudioProfilesService(logger);
        }


        public IEnumerable<TelephonyProfile> GetAudioProfiles(IAdobeConnectProxy provider, ILmsLicense lmsCompany, string principalId)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var usedAudioProfiles = lmsCompany.GetSetting<bool>(LmsCompanySettingNames.AudioProfileUnique) 
                ? _meetingModel.GetByCompanyWithAudioProfiles(lmsCompany).Select(x => x.AudioProfileId).ToList() 
                : new List<string>();
            
            var profiles = _innerService.GetAudioProfiles(provider, principalId)
                .Where(x => !usedAudioProfiles.Contains(x.ProfileId)).ToList();
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

            return _innerService.AddAudioProfileToMeeting(provider, meetingScoId, audioProfileId);
        }

        public OperationResult RemoveAudioProfileFromMeeting(string meetingScoId, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(meetingScoId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingScoId));

            StatusInfo status = provider.UpdateAclField(meetingScoId, AclFieldId.telephony_profile, string.Empty);
            if (status.Code == StatusCodes.ok)
                return OperationResult.Success();

            _logger.ErrorFormat($"Error occured when tried to RemoveAudioProfileFromMeeting. MeetingScoId={meetingScoId}. AC Error: {status.GetErrorInfo()}");
            return OperationResult.Error("Unexpected error. Please refresh page and try again.");
        }
        
        public OperationResult DeleteAudioProfile(string audioProfileId, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrWhiteSpace(audioProfileId))
                throw new ArgumentException("Non-empty value expected", nameof(audioProfileId));

            var deleteProfileStatus = provider.TelephonyProfileDelete(audioProfileId);
            
            return OperationResult.Success();
        }

    }

}