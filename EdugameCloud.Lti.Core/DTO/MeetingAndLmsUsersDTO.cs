using System.Collections.Generic;
using System.Runtime.Serialization;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class MeetingAndLmsUsersDTO
    {
        [DataMember(Name = "meeting")]
        public MeetingDTO meeting { get; set; }

        [DataMember(Name = "lmsUsers")]
        public IEnumerable<LmsUserDTO> lmsUsers { get; set; }

    }

}
