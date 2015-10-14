namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Runtime.Serialization;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    // TODO: move
    [DataContract]
    public class MeetingReuseDTO
    {
        [DataMember]
        public string sco_id { get; set; }

        [DataMember]
        public bool mergeUsers { get; set; }
    }

    public partial class LtiController : Controller
    {
        #region Properties

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }

        private UsersSetup UsersSetup
        {
            get { return IoC.Resolve<UsersSetup>(); }
        }

        private MeetingSetup MeetingSetup
        {
            get { return IoC.Resolve<MeetingSetup>(); }
        }

        #endregion

        [HttpPost]
        public virtual JsonResult ReuseExistedAdobeConnectMeeting(string lmsProviderName, MeetingReuseDTO dto, bool? retrieveLmsUsers)
        {
            if (string.IsNullOrWhiteSpace(lmsProviderName))
                return Json(OperationResult.Error("Session not found."));
            if (string.IsNullOrWhiteSpace(dto.sco_id))
                return Json(OperationResult.Error("Source AdobeConnect meeting is not selected."));

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var provider = this.GetAdobeConnectProvider(credentials);

                OperationResult result = MeetingSetup.ReuseExistedAdobeConnectMeeting(credentials, session.LmsUser,
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
        public virtual JsonResult UpdateMeeting(string lmsProviderName, MeetingDTO meeting)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult ret = this.meetingSetup.SaveMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meeting);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult UpdateMeetingAndReturnLmsUsers(string lmsProviderName, MeetingDTO meeting)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var ret = this.meetingSetup.SaveMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meeting,
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
        public virtual JsonResult DeleteMeeting(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.DeleteMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meetingId);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}