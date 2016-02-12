using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiController : Controller
    {
        [HttpPost]
        public JsonResult GetAudioProfiles(string lmsProviderName, bool notInUse = false)
        {
            LmsCompany lmsCompany = null;
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                credentials = session.LmsCompany;
                var provider = this.GetAdobeConnectProvider(credentials);

                
                var usedAudioProfiles = notInUse ? this.LmsCourseMeetingModel.GetByCompanyWithAudioProfiles(lmsCompany).ToList().Select(x => x.AudioProfileId).ToList() : new List<string>();

                var telephonyPrfilesListResult = provider.TelephonyProfileList(provider.PrincipalId);
                var profiles = telephonyPrfilesListResult.Values.Where(x => !usedAudioProfiles.Contains(x.ProfileId)).ToList();

                return Json(OperationResult.Success(profiles.Select(x => new LmsAudioProfileDTO(x)).ToList()));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAudioProfiles", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public JsonResult AssociateAudioProfileIdWithMeeting(string lmsProviderName, int meetingId, int meetingType, string audioProfileId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var ret = this.meetingSetup.UpdateAudioProfileId(
                    lmsCompany,
                    param,
                    GetAdobeConnectProvider(lmsCompany),
                    meetingId,
                    meetingType,
                    audioProfileId);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AssociateAudioProfileIdWithMeeting", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
    }
}
