namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    // TODO: move
    [DataContract]
    public class MeetingReuseDTO
    {
        [DataMember]
        public string sco_id { get; set; }

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
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var provider = this.GetAdobeConnectProvider(credentials);

                OperationResult result = MeetingSetup.ReuseExistedAdobeConnectMeeting(credentials,
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
        public virtual JsonResult DeleteMeeting(string lmsProviderName, string scoId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.DeleteMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    scoId);

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