using System.Runtime.Serialization;
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    [DataContract]
    public class ZoomMeetingPoolQuestionReport
    {
        [DeserializeAs(Name = "question")]
        [JsonProperty(PropertyName = "question")]
        public string Question { get; set; }

        [DeserializeAs(Name = "answer")]
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
    }
}
