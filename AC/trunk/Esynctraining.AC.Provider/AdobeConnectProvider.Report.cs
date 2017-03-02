using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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

            var doc = this.requestProcessor.Process(Commands.ReportBulkConsolidatedTransactions, 
                string.Format(CommandParams.ReportBulkConsolidatedTransactionsFilters.MeetingScoId, meetingId).AppendPagingIfNeeded(startIndex, limit),
                out status);

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
        
        // Not used from Proxy level
        //public ScoContentCollectionResult ReportRecordings(int startIndex = 0, int limit = 0)
        //{
        //    // act: "report-bulk-objects"
        //    StatusInfo status;

        //    var doc = this.requestProcessor.Process(Commands.ReportBulkObjects, CommandParams.ReportBulkObjectsFilters.Recording.AppendPagingIfNeeded(startIndex, limit), out status);

        //    if (ResponseIsOk(doc, status))
        //    {
        //        var result = new ScoContentCollectionResult(
        //            status, ScoRecordingCollectionParser.Parse(doc));

        //        return result;
        //    }

        //    return new ScoContentCollectionResult(status);
        //}

        //public IEnumerable<ScoContentCollectionResult> ReportRecordingsPaged(int totalLimit = 0, string filter = null, string sort = null)
        //{
        //    // act: "report-bulk-objects" paged by 10K
        //    var status = new StatusInfo();
        //    var responseIsOk = false;
        //    var iteration = 1;
        //    var limit = 10000;
        //    if (totalLimit < limit && totalLimit != 0)
        //        limit = totalLimit;
        //    var scoContentCollectionResult = new ScoContentCollectionResult(status);
        //    var result = new List<ScoContentCollectionResult>();
        //    do
        //    {
        //        var doc = this.requestProcessor.Process(Commands.ReportBulkObjects,
        //            $"{filter}{(filter!=null ? "&" : "")}{CommandParams.ReportBulkObjectsFilters.Recording.AppendPagingIfNeeded((iteration - 1) * limit + 1, limit)}", out status);

        //        responseIsOk = ResponseIsOk(doc, status);
        //        if (responseIsOk)
        //        {
        //            scoContentCollectionResult = new ScoContentCollectionResult(status, ScoRecordingCollectionParser.Parse(doc));
        //            result.Add(scoContentCollectionResult);
        //        }
        //        iteration++;
        //    } while (scoContentCollectionResult.Success 
        //        && scoContentCollectionResult.Values != null
        //        && scoContentCollectionResult.Values.Any() 
        //        && (totalLimit == 0 || totalLimit > 0 && (iteration - 1) * limit < totalLimit));


        //    return result;
        //}

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

        /// <summary>
        /// This API returns a list of all of the possible answers for a question in a quiz, including the response selection item and its description. 
        /// You can also filter the responses to focus on a particular response.
        /// </summary>
        /// <param name="meetingScoId">SCO ID of a training.</param>
        /// <param name="interactionId">Filters by a specific question ID (Interaction ID). 
        /// An interaction is a combination of one question and all of the possible answers.</param>
        public CollectionResult<AssetResponseInfo> ReportAssetResponseInfo(string meetingScoId, string interactionId)
        {
            // act: "report-asset-response-info"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportAssetResponseInfo,
                string.Format(CommandParams.ScoIdInteractionId, meetingScoId, interactionId), out status);

            const string path = "//results/report-asset-responses/response";

            return ResponseIsOk(doc, status)
                ? new CollectionResult<AssetResponseInfo>(status,
                    doc.SelectNodes(path).Cast<XmlNode>()
                    .Select(AssetResponseInfoParser.Parse)
                    .Where(item => item != null)
                    .ToArray())
                : new CollectionResult<AssetResponseInfo>(status);
        }

        public CollectionResult<QuizQuestionResponseItem> ReportQuizQuestionResponse(string meetingScoId)
        {
            // act: "report-quiz-question-response"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportQuizQuestionResponse,
                string.Format(CommandParams.ScoId, meetingScoId.ToString()), out status);

            const string path = "//results/report-quiz-question-response/row";

            return ResponseIsOk(doc, status)
                ? new CollectionResult<QuizQuestionResponseItem>(status,
                    doc.SelectNodes(path).Cast<XmlNode>()
                    .Select(QuizQuestionResponseItemParser.Parse)
                    .Where(item => item != null)
                    .ToArray())
                : new CollectionResult<QuizQuestionResponseItem>(status);
        }

        public CollectionResult<QuizInteractionItem> ReportQuizInteractions(string meetingScoId)
        {
            // act: "report-quiz-interactions
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportQuizInteractions,
                string.Format(CommandParams.ScoId, meetingScoId.ToString()), out status);

            const string path = "//results/report-quiz-interactions/row";

            return ResponseIsOk(doc, status)
                ? new CollectionResult<QuizInteractionItem>(status,
                    doc.SelectNodes(path).Cast<XmlNode>()
                    .Select(QuizInteractionItemParser.Parse)
                    .Where(item => item != null)
                    .ToArray())
                : new CollectionResult<QuizInteractionItem>(status);
        }

        public CollectionResult<QuizQuestionDistributionItem> ReportQuizQuestionDistribution(string meetingScoId)
        {
            // act: "report-quiz-interactions
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportQuizQuestionDistribution,
                string.Format(CommandParams.ScoId, meetingScoId.ToString()), out status);

            const string path = "//results/report-quiz-question-distribution/row";

            return ResponseIsOk(doc, status)
                ? new CollectionResult<QuizQuestionDistributionItem>(status,
                    doc.SelectNodes(path).Cast<XmlNode>()
                    .Select(QuizQuestionDistributionItemParser.Parse)
                    .Where(item => item != null)
                    .ToArray())
                : new CollectionResult<QuizQuestionDistributionItem>(status);
        }

        public CollectionResult<ReportBulkObjectItem> ReportBulkObjects(string filter, int startIndex = 0, int limit = 0)
        {
            return DoCallReportBulkObjects(filter, startIndex, limit);
        }


        private CollectionResult<ReportBulkObjectItem> DoCallReportBulkObjects(string filter, int startIndex, int limit)
        {
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.ReportBulkObjects, filter.AppendPagingIfNeeded(startIndex, limit), out status);
            bool okResponse = ResponseIsOk(principals, status);
            if (!okResponse)
            {
                if (status.Code == StatusCodes.operation_size_error)
                {
                    int? actualAcLimit = status.TryGetSubCodeAsInt32();
                    if (actualAcLimit.HasValue)
                    {
                        return DoCallReportBulkObjects(filter + "&sort-sco-id=asc", startIndex, actualAcLimit.Value);
                    }
                }
                return new CollectionResult<ReportBulkObjectItem>(status);
            }

            const string path = "//results/report-bulk-objects/row";
            List<ReportBulkObjectItem> data = principals.SelectNodes(path).Cast<XmlNode>()
                    .Select(ReportBulkObjectItemParser.Parse)
                    .Where(item => item != null)
                    .ToList();

            if (data.Count() < limit)
                return new CollectionResult<ReportBulkObjectItem>(status, data);

            CollectionResult<ReportBulkObjectItem> nextPage = DoCallReportBulkObjects(filter, startIndex + limit, limit);
            if (!nextPage.Success)
                return nextPage;

            return new CollectionResult<ReportBulkObjectItem>(status, data.Concat(nextPage.Values));
        }

    }

}
