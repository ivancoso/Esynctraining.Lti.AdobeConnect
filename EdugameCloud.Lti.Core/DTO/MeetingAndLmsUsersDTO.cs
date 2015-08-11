using EdugameCloud.Lti.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Lti.Core.DTO
{
    public class MeetingAndLmsUsersDTO
    {
        public MeetingDTO meeting { get; set; }
        public IEnumerable<LmsUserDTO> lmsUsers { get; set; }
    }
}
