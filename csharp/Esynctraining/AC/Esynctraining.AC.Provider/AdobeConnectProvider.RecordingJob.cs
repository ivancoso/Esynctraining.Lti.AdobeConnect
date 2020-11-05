using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        public RecordingJobResult ScheduleRecordingJob(string recordingScoId)
        {
            // act: "schedule-recording-job"
            const string recordingJobPath = "//results/recording-job";

            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Recordings.ScheduleJob, string.Format(CommandParams.SourceScoId, recordingScoId), out status);

            return ResponseIsOk(doc, status)
               ? new RecordingJobResult(status, RecordingJobParser.Parse(doc.SelectSingleNode(recordingJobPath)))
               : new RecordingJobResult(status);
        }

        //public RecordingJobCollectionResult GetRecordingJobsList(string folderId)
        //{
        //    // act: "list-recording-jobs"

        //    StatusInfo status;
        //    var doc = this.requestProcessor.Process(Commands.Recordings.ListJobs, string.Format(CommandParams.FolderId, folderId), out status);

        //    return ResponseIsOk(doc, status)
        //        ? new RecordingJobCollectionResult(status, RecordingJobCollectionParser.Parse(doc))
        //        : new RecordingJobCollectionResult(status);
        //}
        //public RecordingJobResult GetRecordingJob(string jobId)
        //{
        //    // act: "get-recording-job"

        //    StatusInfo status;
        //    const string recordingJobPath = "//results/recording-job";

        //    var doc = this.requestProcessor.Process(Commands.Recordings.GetJob, string.Format(CommandParams.JobId, jobId), out status);

        //    return ResponseIsOk(doc, status)
        //        ? new RecordingJobResult(status, RecordingJobParser.Parse(doc.SelectSingleNode(recordingJobPath)))
        //        : new RecordingJobResult(status);
        //}
        public CancelRecordingJobResult CancelRecordingJob(string jobRecordingScoId)
        {
            //act: "cancel-recording-job"

            StatusInfo status;
            var doc = this.requestProcessor.Process(Commands.Recordings.CancelJob, string.Format(CommandParams.ScoId, jobRecordingScoId), out status);

            return new CancelRecordingJobResult(status);
        }

        public RecordingCollectionResult GetRecordingsList(string folderId, string scoId = null)
        {
            return GetRecordingsList(folderId, 0, 0, null, SortOrder.Unspecified, false, scoId);
        }

        public RecordingCollectionResult GetRecordingsList(string folderId,
            int startIndex, int limit, 
            string propertySortBy, SortOrder order,
            bool excludeMp4 = false,
            string scoId = null)
        {
            //act: "list-recordings"
            StatusInfo status;

            string parameters =
                string.Format(CommandParams.FolderId, folderId)
                .AppendPagingIfNeeded(startIndex, limit)
                .AppendSortingIfNeeded(propertySortBy, order);

            if (excludeMp4)
                parameters += "&filter-out-icon=mp4-archive";

            if (!string.IsNullOrEmpty(scoId))
                parameters += $"&filter-sco-id={scoId}";

            var doc = this.requestProcessor.Process(Commands.Recordings.List, parameters, out status);
            return ResponseIsOk(doc, status)
                ? new RecordingCollectionResult(status, RecordingCollectionParser.Parse(doc))
                : new RecordingCollectionResult(status);
        }


        public RecordingCollectionResult GetSeminarSessionRecordingsList(string seminarId, string seminarSessionId)
        {
            //act: "list-recordings"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Recordings.List, 
                string.Format(CommandParams.FolderIdAndSeminarSessionId, seminarId, seminarSessionId), out status);
            return ResponseIsOk(doc, status)
                ? new RecordingCollectionResult(status, RecordingCollectionParser.Parse(doc))
                : new RecordingCollectionResult(status);
        }

        public GeneratedRecordingJobCollectionResult GetConvertedRecordingsList(string recordingScoId)
        {
            //act: "list-generated-recordings"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Recordings.ListConverted, string.Format(CommandParams.SourceScoId, recordingScoId), out status);
            return ResponseIsOk(doc, status)
                ? new GeneratedRecordingJobCollectionResult(status, GeneratedRecordingJobCollectionParser.Parse(doc))
                : new GeneratedRecordingJobCollectionResult(status);
        }

    }

}
