using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        public TransactionCollectionResult ReportRecordingTransactions(IEnumerable<string> recordingScoIdList, int startIndex = 0, int limit = 0)
        {
            if (recordingScoIdList == null)
                throw new ArgumentNullException(nameof(recordingScoIdList));
            if (!recordingScoIdList.Any())
                throw new ArgumentException("Non-empty list expected", nameof(recordingScoIdList));

            // act: "report-bulk-consolidated-transactions"
            StatusInfo status;

            var parameters = new StringBuilder(CommandParams.ReportBulkConsolidatedTransactionsFilters.TypeRecording);
            foreach (string recordingScoId in recordingScoIdList)
                parameters.AppendFormat(CommandParams.ReportBulkConsolidatedTransactionsFilters.AndByScoId, recordingScoId);

            var doc = this.requestProcessor.Process(Commands.ReportBulkConsolidatedTransactions,
                parameters.ToString().AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                ? new TransactionCollectionResult(status, TransactionInfoCollectionParser.Parse(doc))
                : new TransactionCollectionResult(status);
        }

        /// <summary>
        /// The report meeting transactions.
        /// </summary>
        /// <param name="meetingId">
        /// The meeting id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="TransactionCollectionResult"/>.
        /// </returns>
        public TransactionCollectionResult ReportMeetingTransactions(string meetingId, int startIndex = 0, int limit = 0)
        {
            // act: "report-bulk-consolidated-transactions"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkConsolidatedTransactions, string.Format(CommandParams.ReportBulkConsolidatedTransactionsFilters.MeetingScoId, meetingId).AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                ? new TransactionCollectionResult(status, TransactionInfoCollectionParser.Parse(doc))
                : new TransactionCollectionResult(status);
        }

        /// <summary>
        /// TRICK: uses "sort-date-created=desc".
        /// </summary>        
        public TransactionCollectionResult ReportMeetingTransactionsForPrincipal(string principalId, int startIndex = 0, int limit = 0)
        {
            // act: "report-bulk-consolidated-transactions"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkConsolidatedTransactions,
                string.Format(CommandParams.ReportBulkConsolidatedTransactionsFilters.PrincipalId, principalId)
                .AppendPagingIfNeeded(startIndex, limit), out status);

            return ResponseIsOk(doc, status)
                ? new TransactionCollectionResult(status, TransactionInfoCollectionParser.Parse(doc))
                : new TransactionCollectionResult(status);
        }

        /// <summary>
        /// The get contents by SCO id.
        /// </summary>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentCollectionResult"/>.
        /// </returns>
        public ScoContentCollectionResult ReportRecordings(int startIndex = 0, int limit = 0)
        {
            // act: "report-bulk-objects"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkObjects, CommandParams.ReportBulkObjectsFilters.Recording.AppendPagingIfNeeded(startIndex, limit), out status);

            if (ResponseIsOk(doc, status))
            {
                var result = new ScoContentCollectionResult(
                    status, ScoRecordingCollectionParser.Parse(doc));

                return result;
            }

            return new ScoContentCollectionResult(status);
        }

        public IEnumerable<ScoContentCollectionResult> ReportRecordingsPaged(int totalLimit = 0, string filter = null, string sort = null)
        {
            // act: "report-bulk-objects" paged by 10K
            var status = new StatusInfo();
            var responseIsOk = false;
            var iteration = 1;
            var limit = 10000;
            if (totalLimit < limit && totalLimit != 0)
                limit = totalLimit;
            var scoContentCollectionResult = new ScoContentCollectionResult(status);
            var result = new List<ScoContentCollectionResult>();
            do
            {
                var doc = this.requestProcessor.Process(Commands.ReportBulkObjects,
                    $"{filter}{(filter!=null ? "&" : "")}{CommandParams.ReportBulkObjectsFilters.Recording.AppendPagingIfNeeded((iteration - 1) * limit + 1, limit)}", out status);

                responseIsOk = ResponseIsOk(doc, status);
                if (responseIsOk)
                {
                    scoContentCollectionResult = new ScoContentCollectionResult(status, ScoRecordingCollectionParser.Parse(doc));
                    result.Add(scoContentCollectionResult);
                }
                iteration++;
            } while (scoContentCollectionResult.Success 
                && scoContentCollectionResult.Values != null
                && scoContentCollectionResult.Values.Any() 
                && (totalLimit == 0 || totalLimit > 0 && (iteration - 1) * limit < totalLimit));


            return result;
        }

        public ReportScoViewsContentCollectionResult ReportScoViews(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            // act: "report-sco-views"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportScoViews,
                string.Format(CommandParams.ScoId, scoId), out status);
            
            return ResponseIsOk(doc, status)
                ? new ReportScoViewsContentCollectionResult(status, ReportScoViewCollectionParser.Parse(doc), scoId)
                : new ReportScoViewsContentCollectionResult(status);
        }

        public ReportUserTrainingsTakenCollectionResult ReportUserTrainingTaken(string principalId)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            // act: "report-sco-views"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportUserTrainingsTaken,
                string.Format(CommandParams.PrincipalId, principalId), out status);

            return ResponseIsOk(doc, status)
                ? new ReportUserTrainingsTakenCollectionResult(status, ReportUserTrainingsTakenCollectionParser.Parse(doc), principalId)
                : new ReportUserTrainingsTakenCollectionResult(status);
        }
        
    }

}
