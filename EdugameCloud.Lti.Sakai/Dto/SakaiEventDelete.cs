using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiEventDelete
    {
        [DataMember(Name = "sakaiId")]
        public string SakaiId { get; set; }
    }
}