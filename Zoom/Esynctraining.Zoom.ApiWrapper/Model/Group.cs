using Newtonsoft.Json;
using RestSharp.Deserializers;
using System.Runtime.Serialization;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [DataMember]
        [DeserializeAs(Name = "total_members")]
        [JsonProperty(PropertyName = "total_members")]
        public string TotalMembers { get; set; }
    }
}
