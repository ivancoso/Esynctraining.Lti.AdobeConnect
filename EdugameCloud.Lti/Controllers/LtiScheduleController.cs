namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;

    [HandleError]
    public class LtiScheduleController : Controller
    {
        #region Fields
        
        private readonly LmsCompanyModel _lmsCompanyModel;
        
        private readonly LmsUserSessionModel _lmsSessionModel;
        
        private readonly ScheduleModel _scheduleModel;

        //private readonly IBrainHoneyScheduling _bhScheduling;

        #endregion

        #region Constructors and Destructors
        
        public LtiScheduleController(
            LmsCompanyModel lmsCompanyModel, 
            LmsUserSessionModel lmsSessionModel,
            ScheduleModel scheduleModel)
            //, IBrainHoneyScheduling bhScheduling)
        {
            _lmsCompanyModel = lmsCompanyModel ?? throw new ArgumentNullException(nameof(lmsCompanyModel));
            _lmsSessionModel = lmsSessionModel ?? throw new ArgumentNullException(nameof(lmsSessionModel));
            _scheduleModel = scheduleModel ?? throw new ArgumentNullException(nameof(scheduleModel));
            //_bhScheduling = bhScheduling;
        }

        #endregion
        
        #region Public Methods and Operators

        [HttpGet]
        [ActionName("force-update")]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual ContentResult ForceUpdate(int meetingId)
        {
            string result = null;
            IEnumerable<Schedule> schedules = this._scheduleModel.GetAll();
            foreach (Schedule schedule in schedules)
            {
                Func<IEnumerable<LmsCompany>, DateTime, int, string> scheduledAction = null;

                IEnumerable<LmsCompany> brainHoneyCompanies = null;
                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        //brainHoneyCompanies =
                        //    this.lmsCompanyModel.GetAllByProviderId((int)LmsProviderEnum.BrainHoney);
                        //scheduledAction = _bhScheduling.CheckForBrainHoneySignals;
                        break;
                    case ScheduleDescriptor.CleanLmsSessions:
                        scheduledAction = this.CleanLmsSessions;
                        break;
                }

                string error;
                bool res = this._scheduleModel.ExecuteIfPossible(schedule, scheduledAction, brainHoneyCompanies, meetingId, out error);
                result += "'" + schedule.ScheduleDescriptor
                          + (res ? "' task succedded; Errors: " + error : "' task failed; Errors: " + error);
            }

            return Content(result ?? "Tasks not found");
        }
        
        [HttpGet]
        [ActionName("update-if-necessary")]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual ContentResult UpdateIfNecessary(int meetingId)
        {
            string result = null;
            IEnumerable<Schedule> schedules = this._scheduleModel.GetAll();
            foreach (Schedule schedule in schedules)
            {
                Action<DateTime> scheduledAction = null;

                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        //IEnumerable<LmsCompany> brainHoneyCompanies =
                        //    this._lmsCompanyModel.GetAllByProviderId((int)LmsProviderEnum.BrainHoney);
                        //scheduledAction = dt => _bhScheduling.CheckForBrainHoneySignals(brainHoneyCompanies, dt, meetingId);
                        break;

                    case ScheduleDescriptor.CleanLmsSessions:
                        scheduledAction = dt => this.CleanLmsSessions(null, dt, meetingId);
                        break;
                }

                bool res = this._scheduleModel.ExecuteIfNecessary(schedule, scheduledAction);
                result += "'" + schedule.ScheduleDescriptor + (res ? "' task succedded; " : "' task failed; ");
            }

            return Content(result ?? "Tasks not found");
        }

        #endregion

        #region Methods
        
        private string CleanLmsSessions(IEnumerable<LmsCompany> brainHoneyCompanies, DateTime lastScheduledRunDate, int meetingId = -1)
        {
            IEnumerable<LmsUserSession> lmsSessions = _lmsSessionModel.GetAllOlderThen(DateTime.Now.AddDays(-2));
            foreach (var lmsSession in lmsSessions)
            {
                _lmsSessionModel.RegisterDelete(lmsSession, flush: false);
            }

            _lmsSessionModel.Flush();

            return string.Empty;
        }
        
        #endregion

    }

}