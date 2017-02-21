using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Content.Host.Dto
{
    [DataContract]
    public class ScoShortcutDto
    {
        [DataMember]
        public string ScoId { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Name { get; set; }

    }

}