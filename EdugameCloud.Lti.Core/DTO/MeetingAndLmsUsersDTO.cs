using EdugameCloud.Lti.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Lti.Core.DTO
{
    public class MeetingAndLmsUsersDTO
    {
        public MeetingDTO Meeting { get; set; }
        public IEnumerable<LmsUserDTO> LmsUsers { get; set; }
    }
}
