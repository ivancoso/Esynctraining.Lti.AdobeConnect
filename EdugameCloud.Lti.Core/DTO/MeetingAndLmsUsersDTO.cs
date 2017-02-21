using System.Collections.Generic;
using System.Runtime.Serialization;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class MeetingAndLmsUsersDTO
    {
        [DataMember]
        public MeetingDTO Meeting { get; set; }

        [DataMember]
        public IEnumerable<LmsUserDTO> LmsUsers { get; set; }

    }

}
