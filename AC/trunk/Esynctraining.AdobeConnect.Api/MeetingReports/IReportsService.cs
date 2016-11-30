using System;
using System.Collections.Generic;
using Esynctraining.AdobeConnect.Api.MeetingReports.Dto;

namespace Esynctraining.AdobeConnect.Api.MeetingReports
{
    public interface IReportsService
    {
        IEnumerable<ACSessionDto> GetSessionsReports(string meetingId, IAdobeConnectProxy ac, TimeZoneInfo timeZone,
            int startIndex = 0, int limit = 0);

        IEnumerable<ACSessionParticipantDto> GetAttendanceReports(string meetingId, IAdobeConnectProxy ac, TimeZoneInfo timeZone,
            int startIndex = 0, int limit = 0);
        
    }

}