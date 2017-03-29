using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class LtiReportService
    {
        #region Properties
        
        private ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        #endregion

        #region Public Methods and Operators
        
        public IEnumerable<RecordingTransactionDTO> GetRecordingsReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, 
            int startIndex = 0, int limit = 0)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            try
            {
                var recordingsSco = provider.GetRecordingsList(meeting.GetMeetingScoId()).Values.Select(x => x.ScoId);
                if (!recordingsSco.Any())
                    return Enumerable.Empty<RecordingTransactionDTO>();

                var transactions = provider.ReportRecordingTransactions(recordingsSco, startIndex, limit).Values.ToList();
                return transactions.Select(
                        us =>
                        new RecordingTransactionDTO
                        {
                            RecordingScoId = us.ScoId,
                            RecordingName = us.Name,
                            Login = us.Login,
                            UserName = us.UserName,
                            DateClosed = us.DateClosed != DateTime.MinValue ? us.DateClosed : (DateTime?)null,
                            DateCreated = us.DateCreated,
                        }).OrderByDescending(x => x.DateCreated).ToList();

            }
            catch (Exception ex)
            {
                Logger.Error("GetRecordingsReport.Exception", ex);
                return Enumerable.Empty<RecordingTransactionDTO>();
            }
        }
        
        #endregion
        
    }

}
