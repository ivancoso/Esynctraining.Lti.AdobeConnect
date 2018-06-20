using System.Collections.Generic;

namespace Esynctraining.Lti.Reports.Host.Core.Models
{
    public class SessionReportDto
    {
        public string MeetingUrl { get; set; }
        public string MeetingTitle { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public bool IsExcelFormat { get; set; }
        public string CourseName { get; set; }
        public string LocalDate { get; set; }
        public bool IsShowMeetingTitle { get; set; }

        public List<SessionDto> Sessions { get; set; }
    }
}
