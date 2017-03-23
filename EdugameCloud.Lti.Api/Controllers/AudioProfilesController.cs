using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect.Api.AudioProfiles.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public partial class AudioProfilesController : BaseApiController
    {
        [DataContract]
        public class AudioProfileRequestDto : RequestDto
        {
            [Required]
            [DataMember]
            public int MeetingType { get; set; }

        }

        private IAudioProfilesService AudioProfileService => IoC.Resolve<IAudioProfilesService>();

        private LmsUserModel LmsUserModel => IoC.Resolve<LmsUserModel>();


        public AudioProfilesController(
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
        }


        [Route("audioprofiles")]
        [HttpPost]
        public OperationResultWithData<IEnumerable<AudioProfileDto>> GetAudioProfiles([FromBody]AudioProfileRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var session = Session;

                TelephonyProfileOption option = LmsCompany.GetTelephonyOption((LmsMeetingType)request.MeetingType);
                if (option != TelephonyProfileOption.ReuseExistingProfile)
                {
                    Logger.Error($"TelephonyProfileOption {option} is not ReuseExistingProfile");
                    return Enumerable.Empty<AudioProfileDto>().ToSuccessResult();
                }

                // NOTE: For None option - reuse can be active for every meeting type
                if (((LmsMeetingType)request.MeetingType != LmsMeetingType.OfficeHours)
                    && (LmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.ActiveProfile) != TelephonyDTO.SupportedProfiles.None))
                {
                    Logger.Error($"Meeting type {request.MeetingType} is not supported for audio-profile reuse");
                    return Enumerable.Empty<AudioProfileDto>().ToSuccessResult();
                }

                var provider = this.GetAdminProvider();
                var lmsUser = session.LmsUser ??
                              LmsUserModel.GetOneByUserIdAndCompanyLms(session.LtiSession.LtiParam?.lms_user_id, LmsCompany.Id).Value;
                
                var profiles = AudioProfileService.GetAudioProfiles(provider, LmsCompany, lmsUser.PrincipalId);
                return profiles.Select(x => new AudioProfileDto(x)).ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAudioProfiles", ex);
                return OperationResultWithData<IEnumerable<AudioProfileDto>>.Error(errorMessage);
            }
        }

        //[HttpPost]
        //public JsonResult AssociateAudioProfileIdWithMeeting(string lmsProviderName, int meetingId, int meetingType, string audioProfileId)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;
        //        var param = session.LtiSession.With(x => x.LtiParam);
        //        var provider = GetAdobeConnectProvider(lmsCompany);
        //        var lmsUser = session.LmsUser ??
        //                      lmsUserModel.GetOneByUserIdAndCompanyLms(session.LtiSession.LtiParam?.lms_user_id, lmsCompany.Id).Value;
        //        string principalId = (LmsMeetingType)meetingType == LmsMeetingType.OfficeHours
        //            ? lmsUser.PrincipalId
        //            : provider.PrincipalId;
        //        LmsCourseMeeting meeting = meetingSetup.GetCourseMeeting(lmsCompany, param.course_id, meetingId, meetingType > 0 ? (LmsMeetingType)meetingType : LmsMeetingType.Meeting);

        //        var ret = AudioProfileService.UpdateAudioProfileId(
        //            meeting,
        //            provider,
        //            audioProfileId);

        //        return Json(ret);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("AssociateAudioProfileIdWithMeeting", lmsCompany, ex);
        //        return Json(OperationResult.Error(errorMessage));
        //    }
        //}

    }

}
