using Newtonsoft.Json;
using RestSharp.Deserializers;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ListGroups
    {
        [DataMember]
        [DeserializeAs(Name = "total_records")]
        [JsonProperty(PropertyName = "total_records")]
        public int TotalRecords { get; set; }

        public List<Group> Groups { get; set; }
    }
}
