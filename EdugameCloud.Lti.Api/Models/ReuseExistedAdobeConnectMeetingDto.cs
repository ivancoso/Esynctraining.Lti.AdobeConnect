using System.Runtime.Serialization;
using EdugameCloud.Lti.Models;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class ReuseExistedAdobeConnectMeetingDto : MeetingReuseDTO
    {
        [DataMember]
        public bool? retrieveLmsUsers { get; set; }
    }
}
