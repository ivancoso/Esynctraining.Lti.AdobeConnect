using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiParams
    {
        [DataMember(Name = "lti_message_type")]
        public string LtiMessageType { get; set; }

    }
}