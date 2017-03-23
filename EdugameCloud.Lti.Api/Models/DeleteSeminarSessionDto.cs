using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class DeleteSeminarSessionDto
    {
        [DataMember]
        public string seminarSessionId { get; set; }
    }
}
