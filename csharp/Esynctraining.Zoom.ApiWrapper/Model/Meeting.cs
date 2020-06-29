using System;
using System.Runtime.Serialization;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    [DataContract]
    public class Meeting
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Uuid { get; set; }
        [DataMember]
        public string HostId { get; set; }

        [DataMember]
        public string Topic { get; set; }

        [DataMember]
        public MeetingTypes Type { get; set; }

        [DataMember]
        public DateTimeOffset StartTime { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string Timezone { get; set; }
        [DataMember]
        public MeetingSettings Settings { get; set; }
        [DataMember]
        public MeetingRecurrence Recurrence { get; set; }
        //

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Agenda { get; set; }
        [DataMember]
        public string JoinUrl { get; set; }



    }
}
