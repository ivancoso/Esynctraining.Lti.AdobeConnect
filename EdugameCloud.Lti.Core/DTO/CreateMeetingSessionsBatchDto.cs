using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class CreateMeetingSessionsBatchDto
    {
        [DataMember]
        public int MeetingId { get; set; }

        [Obsolete]
        [DataMember]
        public string StartDate { get; set; }

        [Obsolete]
        [DataMember]
        public string StartTime { get; set; }

        [DataMember]
        public string Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        [DataMember]
        public int[] DaysOfWeek { get; set; }

        [DataMember]
        public int Weeks { get; set; }

        [DataMember]
        public DateTime StartTimestamp { get; set; }
    }
}