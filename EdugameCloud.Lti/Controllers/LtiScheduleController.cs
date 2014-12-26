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
    using EdugameCloud.Lti.Business.Models;
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
        private readonly CompanyLmsModel companyLmsModel;

        /// <summary>
        /// The LMS session model.
        /// </summary>
        private readonly LmsUserSessionModel lmsSessionModel;

        /// <summary>
        ///     The DLAP API.
        /// </summary>
        private readonly DlapAPI dlapApi;

        /// <summary>
        ///     The meeting setup.
        /// </summary>
        private readonly MeetingSetup meetingSetup;

        /// <summary>
        ///     The password activation model.
        /// </summary>
        private readonly ScheduleModel scheduleModel;

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
        /// <param name="companyLmsModel">
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
        public LtiScheduleController(
            DlapAPI dlapApi, 
            MeetingSetup meetingSetup, 
            CompanyLmsModel companyLmsModel, 
            LmsUserSessionModel lmsSessionModel,
            ScheduleModel scheduleModel, 
            ApplicationSettingsProvider settings)
        {
            this.dlapApi = dlapApi;
            this.meetingSetup = meetingSetup;
            this.companyLmsModel = companyLmsModel;
            this.lmsSessionModel = lmsSessionModel;
            this.scheduleModel = scheduleModel;
            this.Settings = settings;
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
                Func<DateTime, string> scheduledAction = null;

                switch (schedule.ScheduleDescriptor)
                {
                    case ScheduleDescriptor.BrainHoneySignals:
                        scheduledAction = this.CheckForBrainHoneySignals;
                        break;
                    case ScheduleDescriptor.CleanLmsSessions:
                        scheduledAction = this.CleanLmsSessions;
                        break;
                }

                string error;
                bool res = this.scheduleModel.ExecuteIfPossible(schedule, scheduledAction, out error);
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
                        scheduledAction = dt => this.CheckForBrainHoneySignals(dt);
                        break;

                    case ScheduleDescriptor.CleanLmsSessions:
                        scheduledAction = dt => this.CleanLmsSessions(dt);
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
        /// The add to errors.
        /// </summary>
        /// <param name="errors">
        /// The errors.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="brainHoneyCompany">
        /// The brain honey company.
        /// </param>
        private void AddToErrors(List<string> errors, string error, CompanyLms brainHoneyCompany)
        {
            errors.Add(string.Format("Error with company {0}:{1}", brainHoneyCompany.With(x => x.LmsDomain), error));
        }

        /// <summary>
        /// The update participants.
        /// </summary>
        /// <param name="lastScheduledRunDate">
        /// The last scheduled run date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        // ReSharper disable once UnusedParameter.Local
        private string CleanLmsSessions(DateTime lastScheduledRunDate)
        {
            IEnumerable<LmsUserSession> lmsSessions = this.lmsSessionModel.GetAllOlderThen(DateTime.Now.AddDays(-7));
            foreach (var lmsSession in lmsSessions)
            {
                this.lmsSessionModel.RegisterDelete(lmsSession);
            }

            this.lmsSessionModel.Flush();

            return string.Empty;
        }

        /// <summary>
        /// The update participants.
        /// </summary>
        /// <param name="lastScheduledRunDate">
        /// The last scheduled run date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string CheckForBrainHoneySignals(DateTime lastScheduledRunDate)
        {
            var errors = new List<string>();
            DlapAPI api = this.dlapApi;
            IEnumerable<CompanyLms> brainHoneyCompanies =
                this.companyLmsModel.GetAllByProviderId((int)LmsProviderEnum.BrainHoney);
            foreach (CompanyLms brainHoneyCompany in brainHoneyCompanies)
            {
                string error;
                Session session = api.BeginBatch(out error, brainHoneyCompany);

                if (error != null)
                {
                    this.AddToErrors(errors, error, brainHoneyCompany);
                }
                else
                {
                    if (brainHoneyCompany.LastSignalId == 0)
                    {
                        long? signalId = api.GetLastSignalId(brainHoneyCompany, out error, session);
                        if (error != null)
                        {
                            this.AddToErrors(errors, error, brainHoneyCompany);
                        }
                        else if (signalId.HasValue)
                        {
                            this.UpdateLastSignalId(brainHoneyCompany, signalId.Value);
                        }

                        continue;
                    }

                    AdobeConnectProvider adobeConnectProvider = this.meetingSetup.GetProvider(brainHoneyCompany);
                    this.meetingSetup.SetupFolders(brainHoneyCompany, adobeConnectProvider);
                    List<TemplateDTO> templates = this.meetingSetup.GetTemplates(
                        adobeConnectProvider, 
                        brainHoneyCompany.ACTemplateScoId);
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
                        List<Signal> signals = api.GetSignalsList(brainHoneyCompany, out error, session);
                        if (error != null)
                        {
                            this.AddToErrors(errors, error, brainHoneyCompany);
                        }
                        else
                        {
                            List<ParsedSignalGroup> parsedSignals = this.Parse(
                                signals.OrderBy(x => x.SignalId).ToList());
                            List<Signal> soleEnrollments =
                                parsedSignals.FirstOrDefault(
                                    x => x.ProcessingSignalType == ProcessingSignalType.SoleEnrollment)
                                    .Return(x => x, new ParsedSignalGroup { SignalsAssociated = new List<Signal>() })
                                    .SignalsAssociated;
                            var userNamesAndEmailsDeleted = new Dictionary<int, List<string>>();
                            foreach (ParsedSignalGroup signalGroup in parsedSignals)
                            {
                                switch (signalGroup.ProcessingSignalType)
                                {
                                    case ProcessingSignalType.CourseToProcess:
                                        List<LmsUserDTO> courseUsers =
                                            this.ProcessCourseCreated(
                                                signalGroup.RepresentativeSignal, 
                                                api, 
                                                brainHoneyCompany, 
                                                session, 
                                                errors, 
                                                adobeConnectProvider, 
                                                templates);
                                        this.RemoveCourseEnrollmentsFromSoleEnrollments(soleEnrollments, courseUsers);
                                        break;

                                    case ProcessingSignalType.ToDelete:
                                        List<string> courseDeletedUsers =
                                            this.ProcessCourseDeleted(
                                                signalGroup.RepresentativeSignal, 
                                                brainHoneyCompany, 
                                                errors, 
                                                adobeConnectProvider);
                                        int key = signalGroup.RepresentativeSignal.EntityId;
                                        if (!userNamesAndEmailsDeleted.ContainsKey(key))
                                        {
                                            userNamesAndEmailsDeleted.Add(key, new List<string>());
                                        }

                                        userNamesAndEmailsDeleted[key].AddRange(courseDeletedUsers);
                                        break;
                                }
                            }

                            this.ProcessSoleEnrollments(
                                brainHoneyCompany, 
                                soleEnrollments, 
                                errors, 
                                api, 
                                session, 
                                adobeConnectProvider);

                            Signal lastSignal = signals.LastOrDefault();
                            if (lastSignal != null)
                            {
                                this.UpdateLastSignalId(brainHoneyCompany, lastSignal.SignalId);
                            }
                        }
                    }
                }
            }

            return errors.ToPlainString();
        }

        /// <summary>
        /// The get group processing signal type.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        /// <returns>
        /// The <see cref="ProcessingSignalType"/>.
        /// </returns>
        private ProcessingSignalType GetGroupProcessingSignalType(List<Signal> @group)
        {
            if (group.Any(x => x.Type == DlapAPI.SignalTypes.CourseDeleted))
            {
                return group.Any(x => x.Type == DlapAPI.SignalTypes.CourseCreated)
                           ? ProcessingSignalType.Skip
                           : ProcessingSignalType.ToDelete;
            }

            return ProcessingSignalType.CourseToProcess;
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="orderedList">
        /// The ordered list.
        /// </param>
        /// <returns>
        /// The <see cref="List{ParsedSignalGroup}"/>.
        /// </returns>
        private List<ParsedSignalGroup> Parse(List<Signal> orderedList)
        {
            var result = new List<ParsedSignalGroup>();
            List<Signal> enrollments = orderedList.Where(x => x.Type == DlapAPI.SignalTypes.EnrollmentChanged).ToList();
            List<Signal> courses = orderedList.Except(enrollments).ToList();
            Dictionary<int, List<Signal>> coursesGrouped = courses.GroupBy(x => x.EntityId)
                .ToDictionary(x => x.Key, x => x.ToList());
            foreach (int key in coursesGrouped.Keys)
            {
                List<Signal> group = coursesGrouped[key];
                result.Add(
                    new ParsedSignalGroup
                        {
                            ProcessingSignalType = this.GetGroupProcessingSignalType(group), 
                            RepresentativeSignal = group.FirstOrDefault(x => x.EntityId != 0), 
                            SignalsAssociated = group
                        });
            }

            if (enrollments.Any())
            {
                result.Add(
                    new ParsedSignalGroup
                        {
                            ProcessingSignalType = ProcessingSignalType.SoleEnrollment, 
                            SignalsAssociated = enrollments
                        });
            }

            return result;
        }

        /// <summary>
        /// The process course created.
        /// </summary>
        /// <param name="signal">
        /// The signal.
        /// </param>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="brainHoneyCompany">
        /// The Brain Honey company.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="errors">
        /// The errors.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe connect provider.
        /// </param>
        /// <param name="templates">
        /// The templates.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        private List<LmsUserDTO> ProcessCourseCreated(
            Signal signal, 
            DlapAPI api, 
            CompanyLms brainHoneyCompany, 
            Session session, 
            List<string> errors, 
            AdobeConnectProvider adobeConnectProvider, 
            IEnumerable<TemplateDTO> templates)
        {
            var result = new List<LmsUserDTO>();
            string error;
            var courseSignal = (CourseSignal)signal;
            Course course = api.GetCourse(brainHoneyCompany, courseSignal.EntityId, out error, session);
            if (error != null)
            {
                this.AddToErrors(errors, error, brainHoneyCompany);
            }
            else
            {
                List<LmsUserDTO> courseEnrolledUsers = api.GetUsersForCourse(
                    brainHoneyCompany, 
                    courseSignal.EntityId, 
                    out error, 
                    session);
                if (error == null && courseEnrolledUsers != null && courseEnrolledUsers.Any())
                {
                    result.AddRange(courseEnrolledUsers);
                }

                DateTime startDate = DateTime.Parse(course.StartDate);
                this.meetingSetup.SaveMeeting(
                    brainHoneyCompany, 
                    adobeConnectProvider, 
                    new LtiParamDTO
                        {
                            context_id = course.CourseId.ToString(CultureInfo.InvariantCulture), 
                            tool_consumer_info_product_family_code = "brainhoney"
                        }, 
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

            return result;
        }

        /// <summary>
        /// The process course created.
        /// </summary>
        /// <param name="signal">
        /// The signal.
        /// </param>
        /// <param name="brainHoneyCompany">
        /// The Brain Honey company.
        /// </param>
        /// <param name="errors">
        /// The errors.
        /// </param>
        /// <param name="adobeConnectProvider">
        /// The adobe connect provider.
        /// </param>
        /// <returns>
        /// The <see cref="List{String}"/>.
        /// </returns>
        private List<string> ProcessCourseDeleted(
            Signal signal, 
            CompanyLms brainHoneyCompany, 
            List<string> errors, 
            AdobeConnectProvider adobeConnectProvider)
        {
            string error;
            List<string> result = this.meetingSetup.DeleteMeeting(
                brainHoneyCompany, 
                adobeConnectProvider, 
                new LtiParamDTO { context_id = signal.EntityId.ToString(CultureInfo.InvariantCulture) }, 
                out error);
            if (error != null)
            {
                this.AddToErrors(errors, error, brainHoneyCompany);
            }

            return result;
        }

        /// <summary>
        /// The process sole enrollments.
        /// </summary>
        /// <param name="brainHoneyCompany">
        /// The brain honey company.
        /// </param>
        /// <param name="soleEnrollments">
        /// The sole enrollments.
        /// </param>
        /// <param name="errors">
        /// The errors.
        /// </param>
        /// <param name="api">
        /// The api.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]

        // ReSharper disable once UnusedParameter.Local
        private void ProcessSoleEnrollments(
            CompanyLms brainHoneyCompany, 
            IEnumerable<Signal> soleEnrollments, 
            List<string> errors, 
            DlapAPI api, 
            Session session, 
            AdobeConnectProvider provider)
        {
            Dictionary<int, IGrouping<int, Signal>> grouped =
                soleEnrollments.OrderBy(x => x.SignalId)
                    .GroupBy(x => x.EntityId, x => x)
                    .ToDictionary(x => x.Key, x => x);
            foreach (int entityId in grouped.Keys)
            {
                EnrollmentSignal latestSignal = grouped[entityId].LastOrDefault().With(x => (EnrollmentSignal)x);
                if (latestSignal != null)
                {
                    if (latestSignal.NewStatus != 0)
                    {
                        string error;
                        Enrollment enrollment = api.GetEnrollment(
                            brainHoneyCompany, 
                            latestSignal.EntityId, 
                            out error, 
                            session);
                        if (error != null)
                        {
                            this.AddToErrors(errors, error, brainHoneyCompany);
                        }
                        else if (enrollment != null)
                        {
                            var lmsUser = new LmsUserDTO
                                              {
                                                  login_id = enrollment.UserName, 
                                                  primary_email = enrollment.Email, 
                                                  lms_role = enrollment.Role, 
                                                  id = enrollment.UserId, 
                                                  is_editable = true, 
                                              };
                            this.meetingSetup.SetLMSUserDefaultACPermissions(provider, null, lmsUser, null);
                            this.meetingSetup.UpdateUser(
                                brainHoneyCompany, 
                                provider, 
                                new LtiParamDTO
                                    {
                                        context_id =
                                            enrollment.CourseId.ToString(CultureInfo.InvariantCulture)
                                    }, 
                                lmsUser, 
                                out error, 
                                true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The remove course enrollments from sole enrollments.
        /// </summary>
        /// <param name="soleEnrollments">
        /// The sole enrollments.
        /// </param>
        /// <param name="courseUsers">
        /// The course users.
        /// </param>
        private void RemoveCourseEnrollmentsFromSoleEnrollments(
            List<Signal> soleEnrollments, 
            List<LmsUserDTO> courseUsers)
        {
            if (courseUsers != null && courseUsers.Any())
            {
                List<int> courseIds = courseUsers.Select(x => int.Parse(x.id)).ToList();
                List<Signal> toRemove = soleEnrollments.Where(x => courseIds.Contains(x.EntityId)).ToList();
                foreach (Signal signal in toRemove)
                {
                    soleEnrollments.Remove(signal);
                }
            }
        }

        /// <summary>
        /// The update last signal id.
        /// </summary>
        /// <param name="brainHoneyCompany">
        /// The brain honey company.
        /// </param>
        /// <param name="signalId">
        /// The signal id.
        /// </param>
        private void UpdateLastSignalId(CompanyLms brainHoneyCompany, long signalId)
        {
            brainHoneyCompany.LastSignalId = signalId;
            this.companyLmsModel.RegisterSave(brainHoneyCompany);
        }

        #endregion
    }
}