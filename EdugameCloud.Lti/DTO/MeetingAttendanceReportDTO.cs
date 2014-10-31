namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The meetings attendance DTO.
    /// </summary>
    [DataContract]
    public class MeetingAttendanceReportDTO
    {
        /// <summary>
        /// Gets or sets the by session.
        /// </summary>
        [DataMember]
        public List<ACSessionDTO> bySession { get; set; }

        /// <summary>
        /// Gets or sets the by attendees.
        /// </summary>
        [DataMember]
        public List<ACSessionParticipantDTO> byAttendees { get; set; }
    }
}
