using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Sakai.Dto
{
    [DataContract]
    public class SakaiExtendedParams
    {
        [DataMember(Name = "lti_message_type")]
        public string LtiMessageType { get; set; }

        [DataMember(Name = "")]
        public string Lti { get; set; }

        //[DataMember(Name = "")]
        //public string Lti { get; set; }

    }
}