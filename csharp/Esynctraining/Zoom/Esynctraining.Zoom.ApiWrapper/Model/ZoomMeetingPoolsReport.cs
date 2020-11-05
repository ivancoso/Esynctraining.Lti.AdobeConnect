using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    [DataContract]
    public class ZoomMeetingPoolsReport
    {
        [DeserializeAs(Name = "id")]
        [JsonProperty(PropertyName = "id")]
        public int MeetingId { get; set; }

        [DeserializeAs(Name = "uuid")]
        [JsonProperty(PropertyName = "uuid")]
        public string MeetingUuid { get; set; }

        [DeserializeAs(Name = "start_time")]
        [JsonProperty(PropertyName = "start_time")]
        public DateTime StartTime { get; set; }

        [DeserializeAs(Name = "questions")]
        [JsonProperty(PropertyName = "questions")]
        public IEnumerable<ZoomUserMeetingQuestionsPoolReport> ZoomUserMeetingQuestionsPoolReports { get; set; }
    }
}
