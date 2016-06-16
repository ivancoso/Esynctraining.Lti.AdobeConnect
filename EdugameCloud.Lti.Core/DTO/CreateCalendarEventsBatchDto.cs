using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class CreateCalendarEventsBatchDto
    {
        [DataMember(Name = "meetingId")]
        public int MeetingId { get; set; }

        [DataMember(Name = "startDate")]
        public string StartDate { get; set; }

        [DataMember(Name = "startTime")]
        public string StartTime { get; set; }

        [DataMember(Name = "duration")]
        public string Duration { get; set; }

        [DataMember(Name = "daysOfWeek")]
        public DayOfWeek[] DaysOfWeek { get; set; }

        [DataMember(Name = "weeks")]
        public int Weeks { get; set; }

        //not used when sending from client side
        [DataMember(Name = "startTimestamp")]
        public long StartTimestamp { get; set; }
    }
}