using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IReportService
    {
        List<ACSessionDTO> GetSessionsReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0);

        List<ACSessionParticipantDTO> GetAttendanceReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0);

        IEnumerable<RecordingTransactionDTO> GetRecordingsReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0);

    }

}
