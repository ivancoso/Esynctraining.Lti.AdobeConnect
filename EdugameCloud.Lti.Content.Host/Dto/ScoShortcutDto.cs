using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Content.Host.Dto
{
    [DataContract]
    public class ScoShortcutDto
    {
        [DataMember(Name = "sco_id")]
        public string ScoId { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

    }

}