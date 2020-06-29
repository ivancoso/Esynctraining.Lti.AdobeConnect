using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Lti.Core.DTO
{
    public class MeetingHostReportDto : List<MeetingHostReportItemDTO>
    {
        public MeetingHostReportDto(IEnumerable<MeetingHostReportItemDTO> items) : base(items)
        {
            
        }
    }
}
