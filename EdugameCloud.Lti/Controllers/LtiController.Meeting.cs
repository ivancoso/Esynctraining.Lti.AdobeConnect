using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    // TODO: move
    [DataContract]
    public class MeetingReuseDTO
    {
        [DataMember]
        public string ScoId { get; set; }

        [DataMember]
        public bool MergeUsers { get; set; }

        [DataMember]
        public int Type { get; set; }


        public LmsMeetingType GetMeetingType()
        {
            if (Type <= 0)
                throw new InvalidOperationException($"Invalid meeting type '{Type}'");
            return (LmsMeetingType)Type;
        }

    }

    public partial class LtiController
    {
        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>(); 


        [HttpPost]
        [LmsAuthorizeBase]
        public virtual JsonResult ReuseExistedAdobeConnectMeeting(LmsUserSession session, MeetingReuseDTO dto, bool? retrieveLmsUsers)
        {
            if (string.IsNullOrWhiteSpace(dto.ScoId))
                return Json(OperationResult.Error("Source AdobeConnect meeting is not selected."));

            LmsCompany credentials = null;
            try
            {
                credentials = session.LmsCompany;

                if (!credentials.GetSetting<bool>(LmsCompanySettingNames.EnableMeetingReuse))
                    return Json(OperationResult.Error("Operation is not enabled."));

                var param = session.LtiSession.With(x => x.LtiParam);
                var provider = this.GetAdminProvider(credentials);

                OperationResult result = meetingSetup.ReuseExistedAdobeConnectMeeting(credentials, session.LmsUser,
                    provider,
                    param,
                    dto,
                    retrieveLmsUsers.GetValueOrDefault());

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ReuseExistedAdobeConnecMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorizeBase]
        public virtual JsonResult UpdateMeeting(LmsUserSession session, MeetingDTOInput meeting)
        {
            LmsCompany credentials = null;
            try
            {
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var trace = new StringBuilder();

                var ac = this.GetAdminProvider(credentials);
                var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);
                var fb = new MeetingFolderBuilder(credentials, ac, useLmsUserEmailForSearch, meeting.GetMeetingType());

                OperationResult ret = this.meetingSetup.SaveMeeting(
                    credentials,
                    ac,
                    param,
                    meeting,
                    trace,
                    fb);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorizeBase]
        public virtual JsonResult UpdateMeetingAndReturnLmsUsers(LmsUserSession session, MeetingDTOInput meeting)
        {
            LmsCompany credentials = null;
            try
            {
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var trace = new StringBuilder();

                var ac = this.GetAdminProvider(credentials);
                var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);
                var fb = new MeetingFolderBuilder(credentials, ac, useLmsUserEmailForSearch, meeting.GetMeetingType());

                var ret = this.meetingSetup.SaveMeeting(
                    credentials,
                    ac,
                    param,
                    meeting,
                    trace,
                    fb,
                    true);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeetingAndReturnLmsUsers", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorizeBase]
        public virtual JsonResult DeleteMeeting(LmsUserSession session, int meetingId, bool? remove = false)
        {
            bool? softDelete = remove;
            LmsCompany lmsCompany = null;
            try
            {
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                
                OperationResult result = this.meetingSetup.DeleteMeeting(
                    lmsCompany,
                    this.GetAdminProvider(lmsCompany),
                    param,
                    meetingId, softDelete.GetValueOrDefault());

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteMeeting", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}