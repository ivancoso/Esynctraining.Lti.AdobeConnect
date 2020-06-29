using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    [DataContract]
    public class ZoomUserMeetingQuestionsPoolReport
    {
        [DeserializeAs(Name = "name")]
        [JsonProperty(PropertyName = "name")]
        public string UserName { get; set; }

        [DeserializeAs(Name = "question_details")]
        [JsonProperty(PropertyName = "question_details")]
        public IEnumerable<ZoomMeetingPoolQuestionReport> ZoomMeetingPoolQuestions { get; set; }
    }
}
