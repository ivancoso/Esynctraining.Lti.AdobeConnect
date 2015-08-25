namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    /// <summary>
    ///     The schedule controller.
    /// </summary>
    [HandleError]
    public class LtiScheduleController : Controller
    {
        #region Fields

        /// <summary>
        ///     The company LMS model.
        /// </summary>
        private readonly LmsCompanyModel lmsCompanyModel;

        /// <summary>
        /// The LMS session model.
        /// </summary>
        private readonly LmsUserSessionModel lmsSessionModel;

        /// <summary>
        ///     The DLAP API.
        /// </summary>
        private readonly IBrainHoneyApi dlapApi;

        /// <summary>
        ///     The meeting setup.
        /// </summary>
        private readonly MeetingSetup meetingSetup;

        /// <summary>
        /// The users setup.
        /// </summary>
        private readonly UsersSetup usersSetup;

        /// <summary>
        ///     The password activation model.
        /// </summary>
        private readonly ScheduleModel scheduleModel;

        private readonly IBrainHoneyScheduling _bhScheduling;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LtiScheduleController"/> class.
        /// </summary>
        /// <param name="dlapApi">
        /// The DLAP API.
        /// </param>
        /// <param name="meetingSetup">
        /// The meeting Setup.
        /// </param>
        /// <param name="lmsCompanyModel">
        /// The company LMS Model.
        /// </param>
        /// <param name="lmsSessionModel">
        /// The LMS Session Model.
        /// </param>
        /// <param name="scheduleModel">
        /// The schedule Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        /// <param name="usersSetup">
        /// The users setup.
        /// </param>
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

        /// <summary>
        ///     Gets the settings.
        /// </summary>
        public dynamic Settings { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The force update.
        /// </summary>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
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

        /// <summary>
        ///     The index.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
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

        /// <summary>
        /// The update participants.
        /// </summary>
        /// <param name="lastScheduledRunDate">
        /// The last scheduled run date.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        // ReSharper disable once UnusedParameter.Local
        private string CleanLmsSessions(IEnumerable<LmsCompany> brainHoneyCompanies, DateTime lastScheduledRunDate, int meetingId = -1)
        {
            IEnumerable<LmsUserSession> lmsSessions = this.lmsSessionModel.GetAllOlderThen(DateTime.Now.AddDays(-7));
            foreach (var lmsSession in lmsSessions)
            {
                this.lmsSessionModel.RegisterDelete(lmsSession);
            }

            this.lmsSessionModel.Flush();

            return string.Empty;
        }
        
        #endregion

    }

}