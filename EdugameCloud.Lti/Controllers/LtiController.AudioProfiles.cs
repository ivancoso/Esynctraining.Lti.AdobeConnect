using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiController : Controller
    {
        private IAudioProfilesService AudioProfileService
        {
            get { return IoC.Resolve<IAudioProfilesService>(); }
        }

        [HttpPost]
        public JsonResult GetAudioProfiles(string lmsProviderName, int meetingType)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                TelephonyProfileOption option = lmsCompany.GetTelephonyOption((LmsMeetingType)meetingType);
                if (option != TelephonyProfileOption.ReuseExistingProfile)
                {
                    logger.Error($"TelephonyProfileOption {option} is not ReuseExistingProfile");
                    return Json(OperationResultWithData<IEnumerable<LmsAudioProfileDTO>>.Success(Enumerable.Empty<LmsAudioProfileDTO>()));
                }

                // NOTE: For None option - reuse can be active for every meeting type
                if (((LmsMeetingType)meetingType != LmsMeetingType.OfficeHours)
                    && (lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.ActiveProfile) != TelephonyDTO.SupportedProfiles.None))
                {
                    logger.Error($"Meeting type {meetingType} is not supported for audio-profile reuse");
                    return Json(OperationResultWithData<IEnumerable<LmsAudioProfileDTO>>.Success(Enumerable.Empty<LmsAudioProfileDTO>()));
                }

                var provider = this.GetAdobeConnectProvider(lmsCompany);
                var lmsUser = session.LmsUser ??
                              lmsUserModel.GetOneByUserIdAndCompanyLms(session.LtiSession.LtiParam?.lms_user_id, lmsCompany.Id).Value;

                string principalId = null;

                // TRICK: old ugly logic. Nodoby know why should it work this way.
                if (lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.ActiveProfile) == TelephonyDTO.SupportedProfiles.None)
                {
                     principalId = (LmsMeetingType)meetingType == LmsMeetingType.OfficeHours
                        ? lmsUser.PrincipalId
                        : provider.PrincipalId;
                }
                else
                {
                    principalId = lmsUser.PrincipalId;
                }
                
                var profiles = AudioProfileService.GetAudioProfiles(provider, lmsCompany, principalId);
                return Json(OperationResultWithData<IEnumerable<LmsAudioProfileDTO>>.Success(profiles.Select(x => new LmsAudioProfileDTO(x)).ToList()));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAudioProfiles", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
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
