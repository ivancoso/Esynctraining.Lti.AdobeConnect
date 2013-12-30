namespace DepositionConferencing.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using DepositionConferencing.Core.Business.Models;
    using DepositionConferencing.Core.Domain.Entities;

    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Providers;

    /// <summary>
    ///     The schedule controller.
    /// </summary>
    [HandleError]
    public partial class ScheduleController : BaseController
    {
        #region Fields

        /// <summary>
        /// The AC session model.
        /// </summary>
        private readonly ACSessionModel sessionModel;

        /// <summary>
        /// The ac session participant model.
        /// </summary>
        private readonly ACSessionParticipantModel sessionParticipantModel;

        /// <summary>
        /// The contact model.
        /// </summary>
        private readonly ContactModel contactModel;

        /// <summary>
        /// The integration model.
        /// </summary>
        private readonly ACIntegrationModel integrationModel;

        /// <summary>
        ///     The password activation model.
        /// </summary>
        private readonly ScheduleModel scheduleModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleController"/> class.
        /// </summary>
        /// <param name="sessionModel">
        /// The ac Session Model.
        /// </param>
        /// <param name="sessionParticipantModel">
        /// The ac Session Participant Model.
        /// </param>
        /// <param name="integrationModel">
        /// The integration Model.
        /// </param>
        /// <param name="contactModel">
        /// The contact Model.
        /// </param>
        /// <param name="scheduleModel">
        /// The password Activation Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        public ScheduleController(
            ACSessionModel sessionModel, 
            ACSessionParticipantModel sessionParticipantModel, 
            ACIntegrationModel integrationModel, 
            ContactModel contactModel, 
            ScheduleModel scheduleModel, 
            ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.sessionModel = sessionModel;
            this.sessionParticipantModel = sessionParticipantModel;
            this.integrationModel = integrationModel;
            this.contactModel = contactModel;
            this.scheduleModel = scheduleModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The force update.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
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
                    case ScheduleDescriptor.UpdateParticipants:
                        scheduledAction = this.UpdateParticipants;
                        break;
                }

                var res = this.scheduleModel.ExecuteIfPossible(schedule, scheduledAction);
                result += "'" + schedule.ScheduleDescriptor.ToString() + (res ? "' task succedded; " : "' task failed; ");
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
                    case ScheduleDescriptor.UpdateParticipants:
                        scheduledAction = this.UpdateParticipants;
                        break;
                }

                var res = this.scheduleModel.ExecuteIfNecessary(schedule, scheduledAction);
                result += "'" + schedule.ScheduleDescriptor.ToString() + (res ? "' task succedded; " : "' task failed; ");
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
        private void UpdateParticipants(DateTime lastScheduledRunDate)
        {
            List<ACSession> meetings = this.sessionModel.GetAll().ToList();
            IEnumerable<int> mettingsScoIds = meetings.Select(x => x.ScoId);
            foreach (int mettingsScoId in mettingsScoIds)
            {
                ACSession meeting = meetings.FirstOrDefault(x => x.ScoId == mettingsScoId);
                if (meeting != null)
                {
                    foreach (ACSessionParticipant participant in meeting.Participants)
                    {
                        this.sessionParticipantModel.RegisterDelete(participant);
                    }

                    meeting.Participants.Clear();
                    IEnumerable<MeetingAttendee> participants =
                        this.integrationModel.GetUpdatesForMeeting(mettingsScoId.ToString());
                    foreach (MeetingAttendee meetingAttendee in participants)
                    {
                        Contact user;
                        if ((user = this.contactModel.GetOneByEmail(meetingAttendee.Login).Value) != null
                            || (user = this.contactModel.GetOneByPrincipalId(meetingAttendee.PrincipalId).Value) != null)
                        {
                            if (user.ContactType.Id == (int)ContactTypeEnum.LegalParticipant)
                            {
                                var participant = new ACSessionParticipant
                                                      {
                                                          ACSession = meeting,
                                                          Contact = user,
                                                          DateTimeEntered =
                                                              meetingAttendee.DateCreated,
                                                          DateTimeLeft = meetingAttendee.DateEnd
                                                      };
                                this.sessionParticipantModel.RegisterSave(participant);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}