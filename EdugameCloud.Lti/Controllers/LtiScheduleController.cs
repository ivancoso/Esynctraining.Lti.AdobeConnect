namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.Core.Providers;

    /// <summary>
    /// The schedule controller.
    /// </summary>
    [HandleError]
    public class LtiScheduleController : Controller
    {
        #region Fields
        
        private readonly LmsCompanyModel lmsCompanyModel;
        
        private readonly LmsUserSessionModel lmsSessionModel;
        
        private readonly IBrainHoneyApi dlapApi;
        
        private readonly MeetingSetup meetingSetup;
        
        private readonly UsersSetup usersSetup;
        
        private readonly ScheduleModel scheduleModel;

        private readonly IBrainHoneyScheduling _bhScheduling;

        #endregion

        #region Constructors and Destructors
        
        public LtiScheduleController(
            IBrainHoneyApi dlapApi, 
            MeetingSetup meetingSetup, 
            LmsCompanyModel lmsCompanyModel, 
            LmsUserSessionModel lmsSessionModel,
            ScheduleModel scheduleModel, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IBrainHoneyScheduling bhScheduling)
        {
            this.dlapApi = dlapApi;
            this.meetingSetup = meetingSetup;
            this.lmsCompanyModel = lmsCompanyModel;
            this.lmsSessionModel = lmsSessionModel;
            this.scheduleModel = scheduleModel;
            this.Settings = settings;
            this.usersSetup = usersSetup;
            _bhScheduling = bhScheduling;
        }

        #endregion

        #region Public Properties
        
        public dynamic Settings { get; private set; }

        #endregion

        protected override JsonResult Json(object data, string contentType,
                System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
            };
        }

        #region Public Methods and Operators

        [HttpGet]
        [ActionName("force-update")]
        public virtual ActionResult ForceUpdate(int meetingId)
        {
            string result = null;
            IEnumerable<Schedule> schedules = this.scheduleModel.GetAll();
            foreach (Schedule schedule in schedules)
            {
                Func<IEnumerable<LmsCompany>, DateTime, int, string> scheduledAction = null;

                IEnumerable<LmsCompany> brainHoneyCompanies = null;
                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        brainHoneyCompanies =
                            this.lmsCompanyModel.GetAllByProviderId((int)LmsProviderEnum.BrainHoney);
                        scheduledAction = _bhScheduling.CheckForBrainHoneySignals;
                        break;
                    case ScheduleDescriptor.CleanLmsSessions:
                        scheduledAction = this.CleanLmsSessions;
                        break;
                }

                string error;
                bool res = this.scheduleModel.ExecuteIfPossible(schedule, scheduledAction, brainHoneyCompanies, meetingId, out error);
                result += "'" + schedule.ScheduleDescriptor
                          + (res ? "' task succedded; Errors: " + error : "' task failed; Errors: " + error);
            }

            return this.Content(result ?? "Tasks not found");
        }
        
        [HttpGet]
        [ActionName("update-if-necessary")]
        public virtual ActionResult UpdateIfNecessary(int meetingId)
        {
            string result = null;
            IEnumerable<Schedule> schedules = this.scheduleModel.GetAll();
            foreach (Schedule schedule in schedules)
            {
                Action<DateTime> scheduledAction = null;

                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        IEnumerable<LmsCompany> brainHoneyCompanies =
                            this.lmsCompanyModel.GetAllByProviderId((int)LmsProviderEnum.BrainHoney);
                        scheduledAction = dt => _bhScheduling.CheckForBrainHoneySignals(brainHoneyCompanies, dt, meetingId);
                        break;

                    case ScheduleDescriptor.CleanLmsSessions:
                        scheduledAction = dt => this.CleanLmsSessions(null, dt, meetingId);
                        break;
                }

                bool res = this.scheduleModel.ExecuteIfNecessary(schedule, scheduledAction);
                result += "'" + schedule.ScheduleDescriptor + (res ? "' task succedded; " : "' task failed; ");
            }

            return this.Content(result ?? "Tasks not found");
        }

        #endregion

        #region Methods
        
        private string CleanLmsSessions(IEnumerable<LmsCompany> brainHoneyCompanies, DateTime lastScheduledRunDate, int meetingId = -1)
        {
            IEnumerable<LmsUserSession> lmsSessions = this.lmsSessionModel.GetAllOlderThen(DateTime.Now.AddDays(-2));
            foreach (var lmsSession in lmsSessions)
            {
                this.lmsSessionModel.RegisterDelete(lmsSession, flush: false);
            }

            this.lmsSessionModel.Flush();

            return string.Empty;
        }
        
        #endregion

    }

}