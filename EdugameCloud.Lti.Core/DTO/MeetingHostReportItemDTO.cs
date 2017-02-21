using System;

namespace EdugameCloud.Lti.Core.DTO
{
    public class MeetingHostReportItemDTO
    {
        public string HostName { get; set; }
        public string Email { get; set; }
        public string MeetingName { get; set; }
        public DateTime? LastMeetingAttend { get; set; }

        public MeetingHostReportItemDTO()
        {

        }

        public MeetingHostReportItemDTO(PrincipalReportDto principalReport)
        {
            this.HostName = principalReport.Principal.Name;
            this.Email = principalReport.Principal.Email;
            this.MeetingName = principalReport.LastTransaction == null ? string.Empty : principalReport.LastTransaction.Name;
            this.LastMeetingAttend = principalReport.LastTransaction == null ? null : (DateTime?)principalReport.LastTransaction.DateCreated;
        }
    }
}