using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class MeetingsController : BaseApiController
    {
        private readonly MeetingSetup meetingSetup;

        #region Constructors and Destructors

        public MeetingsController(
            MeetingSetup meetingSetup,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            this.meetingSetup = meetingSetup;
        }

        #endregion

        [Route("meetings")]
        [HttpPost]
        [Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<MeetingDTO>> GetCourseMeetings()
        {
            StringBuilder trace = null;
            var acProvider = this.GetAdminProvider();

            // TODO: implement. will be use be External API only
            IEnumerable<MeetingDTO> meetings = meetingSetup.GetMeetings(
                   LmsCompany,
                   CourseId,
                   acProvider,
                   new LmsUser(),
                    new LtiParamDTO(),
                   //session.LmsUser,
                   //param,
                   trace);
            return meetings.ToSuccessResult();

            return Enumerable.Empty<MeetingDTO>().ToSuccessResult();
        }

        [Route("meeting/update")]
        [HttpPost]
        [Filters.LmsAuthorizeBase]
        public virtual OperationResult UpdateMeeting([FromBody]MeetingDTOInput meeting)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();

                var ac = this.GetAdminProvider();
                var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);
                var fb = new MeetingFolderBuilder(Session.LmsCompany, ac, useLmsUserEmailForSearch, meeting.GetMeetingType());

                OperationResult ret = this.meetingSetup.SaveMeeting(
                    LmsCompany,
                    ac,
                    param,
                    meeting,
                    trace,
                    fb);

                return ret;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeeting", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("meeting/UpdateAndReturnLmsUsers")]
        [HttpPost]
        [Filters.LmsAuthorizeBase]
        public virtual OperationResult UpdateMeetingAndReturnLmsUsers([FromBody]MeetingDTOInput meeting)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();

                var ac = this.GetAdminProvider();
                var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);
                var fb = new MeetingFolderBuilder(Session.LmsCompany, ac, useLmsUserEmailForSearch, meeting.GetMeetingType());

                var ret = this.meetingSetup.SaveMeeting(
                    LmsCompany,
                    ac,
                    param,
                    meeting,
                    trace,
                    fb,
                    true);

                return ret;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeetingAndReturnLmsUsers", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("meeting/delete")]
        [HttpPost]
        [Filters.LmsAuthorizeBase]
        public virtual OperationResult DeleteMeeting([FromBody]DeleteMeetingDto model)
        {
            bool? softDelete = model.Remove;
            try
            {
                var param = Session.LtiSession.LtiParam;

                OperationResult result = this.meetingSetup.DeleteMeeting(
                    LmsCompany,
                    this.GetAdminProvider(),
                    param,
                    model.MeetingId, softDelete.GetValueOrDefault());

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteMeeting", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("useExistingMeeting")]
        [HttpPost]
        [Filters.LmsAuthorizeBase(FeatureName = LmsCompanySettingNames.EnableMeetingReuse)]
        public virtual OperationResult ReuseExistedAdobeConnectMeeting([FromBody]ReuseExistedAdobeConnectMeetingDto model)
        {
            if (string.IsNullOrWhiteSpace(model.ScoId))
                return OperationResult.Error("Source AdobeConnect meeting is not selected.");

            try
            {
                var param = Session.LtiSession.LtiParam;
                var provider = this.GetAdminProvider();

                OperationResult result = meetingSetup.ReuseExistedAdobeConnectMeeting(LmsCompany, Session.LmsUser,
                    provider,
                    param,
                    model,
                    model.RetrieveLmsUsers.GetValueOrDefault());

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ReuseExistedAdobeConnecMeeting", ex);
                return OperationResult.Error(errorMessage);
            }
        }

    }

}