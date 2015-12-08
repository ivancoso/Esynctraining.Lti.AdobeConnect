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

        /// <summary>
        /// Gets or sets the company LMS.
        /// </summary>
        public virtual int LmsCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        public virtual int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        public virtual string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the lms meeting type.
        /// </summary>
        public virtual int LmsMeetingType { get; set; }

        /// <summary>
        /// Gets or sets the office hours.
        /// </summary>
        public virtual OfficeHours OfficeHours { get; set; }

        public virtual bool? Reused { get; set; }

        public virtual int? SourceCourseMeetingId { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        public virtual LmsUser Owner { get; set; }

        public virtual string MeetingNameJson { get; set; }

        public virtual IList<LmsUserMeetingRole> MeetingRoles { get; protected set; }

        public virtual IList<LmsCourseMeetingGuest> MeetingGuests { get; protected set; }

        public virtual IList<LmsCourseMeetingRecording> MeetingRecordings { get; protected set; }

        #endregion

        public LmsCourseMeeting()
        {
            MeetingRoles = new List<LmsUserMeetingRole>();
            MeetingGuests = new List<LmsCourseMeetingGuest>();
            MeetingRecordings = new List<LmsCourseMeetingRecording>();
        }

    }

}