using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class CreateMeetingSessionsBatchDto
    {
        [DataMember(Name = "meetingId")]
        public int MeetingId { get; set; }

        [DataMember(Name = "startDate")]
        public string StartDate { get; set; }

        [DataMember(Name = "startTime")]
        public string StartTime { get; set; }

        [DataMember(Name = "duration")]
        public string Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        [DataMember(Name = "daysOfWeek")]
        public int[] DaysOfWeek { get; set; }

        [DataMember(Name = "weeks")]
        public int Weeks { get; set; }

        //not used when sending from client side
        [DataMember(Name = "startTimestamp")]
        public long StartTimestamp { get; set; }
    }
}