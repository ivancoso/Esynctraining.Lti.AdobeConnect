using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdugameClaud.Lti.SearchApi.Host.DTOs
{
    public class MeetingNameInfoDto
    {
        public string courseId { get; set; }

        public string courseNum { get; set; }

        public string courseName { get; set; }

        public string meetingName { get; set; }

        public string date { get; set; }

        public string reusedMeetingName { get; set; }
    }
}
