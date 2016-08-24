namespace EdugameCloud.Lti.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Domain.Entities;

    /// <summary>
    /// The LMS AC meeting.
    /// </summary>
    public class LmsCourseMeeting : Entity
    {
        #region Public Properties

        public virtual int LmsCompanyId { get; set; }

        public virtual int CourseId { get; set; }

        public virtual string ScoId { get; set; }

        public virtual int LmsMeetingType { get; set; }

        public virtual OfficeHours OfficeHours { get; set; }

        public virtual bool? Reused { get; set; }

        public virtual int? SourceCourseMeetingId { get; set; }

        public virtual string AudioProfileId { get; set; }

        /// <summary>
        /// If non empty - represents provider profile was generated for.
        /// </summary>
        public virtual string AudioProfileProvider { get; set; }

        public virtual LmsUser Owner { get; set; }

        public virtual string MeetingNameJson { get; set; }

        public virtual IList<LmsUserMeetingRole> MeetingRoles { get; protected set; }

        public virtual IList<LmsCourseMeetingGuest> MeetingGuests { get; protected set; }

        public virtual IList<LmsCourseMeetingRecording> MeetingRecordings { get; protected set; }

        public virtual IList<LmsCalendarEvent> CalendarEvents { get; protected set; }

        public virtual bool EnableDynamicProvisioning { get; set; }
        
        #endregion

        public LmsCourseMeeting()
        {
            MeetingRoles = new List<LmsUserMeetingRole>();
            MeetingGuests = new List<LmsCourseMeetingGuest>();
            MeetingRecordings = new List<LmsCourseMeetingRecording>();
            CalendarEvents = new List<LmsCalendarEvent>();
        }

    }

}