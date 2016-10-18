using System;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Host.Dtos
{
    public class ReportParamsDto
    {
        public ReportParamsDto(string format, string acMeetingUrl, string acMeetingTitle, DateTime localDate, bool isShowMeetingTitle)
        {
            this.format = format;
            this.acMeetingTitle = acMeetingTitle;
            this.acMeetingUrl = acMeetingUrl;
            this.localDate = localDate;
            this.isShowMeetingTitle = isShowMeetingTitle;
        }

        public ReportParamsDto(string format, Company company, LmsUserSession userSession, string acMeetingUrl, string acMeetingTitle, DateTime localDate, bool isShowMeetingTitle)
        {
            this.format = format;
            this.acMeetingTitle = acMeetingTitle;
            this.acMeetingUrl = acMeetingUrl;
            this.localDate = localDate;
            this.isShowMeetingTitle = isShowMeetingTitle;
            this.company =company;
            this.userSession = userSession;
        }
        public string format { get; set; }
        public string acMeetingUrl { get; set; }
        public string acMeetingTitle { get; set; }
        public DateTime localDate { get; set; }
        public bool isShowMeetingTitle { get; set; }

        public Company company { get; set; }

        public LmsUserSession userSession { get; set; }
    }
}