namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    /// <summary>
    ///     The schedule controller.
    /// </summary>
    [HandleError]
    public class ScheduleController : BaseController
    {
        #region Fields

        /// <summary>
        /// The DLAP API.
        /// </summary>
        private readonly DlapAPI dlapApi;

        private readonly MeetingSetup meetingSetup;

        private readonly CompanyLmsModel companyLmsModel;

        /// <summary>
        ///     The password activation model.
        /// </summary>
        private readonly ScheduleModel scheduleModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleController"/> class.
        /// </summary>
        /// <param name="dlapApi">
        /// The DLAP API.
        /// </param>
        /// <param name="meetingSetup">
        /// The meeting Setup.
        /// </param>
        /// <param name="companyLmsModel">
        /// The company LMS Model.
        /// </param>
        /// <param name="scheduleModel">
        /// The schedule Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        public ScheduleController(
            DlapAPI dlapApi,
            MeetingSetup meetingSetup,
            CompanyLmsModel companyLmsModel,
            ScheduleModel scheduleModel, 
            ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.dlapApi = dlapApi;
            this.meetingSetup = meetingSetup;
            this.companyLmsModel = companyLmsModel;
            this.scheduleModel = scheduleModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The force update.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [HttpGet]
        [ActionName("force-update")]
        public virtual ActionResult ForceUpdate()
        {
            string result = null;
            IEnumerable<Schedule> schedules = this.scheduleModel.GetAll();
            foreach (Schedule schedule in schedules)
            {
                Action<DateTime> scheduledAction = null;

                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        scheduledAction = this.CheckForBrainHoneySignals;
                        break;
                }

                var res = this.scheduleModel.ExecuteIfPossible(schedule, scheduledAction);
                result += "'" + schedule.ScheduleDescriptor.ToString()
                          + (res ? "' task succedded; " : "' task failed; ");
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
        public virtual ActionResult UpdateIfNecessary()
        {
            string result = null;
            IEnumerable<Schedule> schedules = this.scheduleModel.GetAll();
            foreach (Schedule schedule in schedules)
            {
                Action<DateTime> scheduledAction = null;

                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        scheduledAction = this.CheckForBrainHoneySignals;
                        break;
                }

                var res = this.scheduleModel.ExecuteIfNecessary(schedule, scheduledAction);
                result += "'" + schedule.ScheduleDescriptor.ToString()
                          + (res ? "' task succedded; " : "' task failed; ");
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
        [NonAction]
        private void CheckForBrainHoneySignals(DateTime lastScheduledRunDate)
        {
            var errors = new List<string>();
            var api = this.dlapApi;
            var brainHoneyCompanies = this.companyLmsModel.GetAllByProviderId((int)LmsProviderEnum.BrainHoney);
            foreach (var brainHoneyCompany in brainHoneyCompanies)
            {
                string error;
                var session = api.BeginBatch(out error, brainHoneyCompany);
                var adobeConnectProvider = this.meetingSetup.GetProvider(brainHoneyCompany);
                this.meetingSetup.SetupFolders(brainHoneyCompany, adobeConnectProvider);
                var templates = this.meetingSetup.GetTemplates(adobeConnectProvider, brainHoneyCompany.ACTemplateScoId);
                if (!templates.Any())
                {
                    error = "No templates found for " + brainHoneyCompany.LmsDomain;
                }
                if (error != null)
                {
                    this.AddToErrors(errors, error, brainHoneyCompany);
                }
                else
                {
                    var signals = api.GetSignalsList(brainHoneyCompany, out error, session);
                    if (error != null)
                    {
                        this.AddToErrors(errors, error, brainHoneyCompany);
                    }
                    else
                    {
                        
                        signals = signals.OrderByDescending(x => x.SignalId).ToList();
                        foreach (var signal in signals)
                        {
                            if (signal.Type == DlapAPI.SignalTypes.CourseCreated)
                            {
                                var courseSignal = (CourseSignal)signal;
                                var course = api.GetCourse(brainHoneyCompany, courseSignal.EntityId, out error, session);
                                if (error != null)
                                {
                                    this.AddToErrors(errors, error, brainHoneyCompany);
                                }
                                else
                                {
                                    var startDate = DateTime.Parse(course.StartDate);
                                    this.meetingSetup.SaveMeeting(
                                        brainHoneyCompany,
                                        adobeConnectProvider,
                                        new LtiParamDTO { context_id = courseSignal.ItemId },
                                        new MeetingDTO
                                            {
                                                start_date = startDate.ToString("MM-dd-yyyy"),
                                                start_time = startDate.ToString("hh:mm tt"),
                                                duration = "01:00",
                                                id = courseSignal.ItemId,
                                                name = course.Title,
                                                template = templates.First().With(x => x.id)
                                            },
                                            session);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddToErrors(List<string> errors, string error, CompanyLms brainHoneyCompany)
        {
            errors.Add(string.Format("Error with company {0}:{1}", brainHoneyCompany.With(x => x.LmsDomain), error));
        }

        #endregion
    }
}