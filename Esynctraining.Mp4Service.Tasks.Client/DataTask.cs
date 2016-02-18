using System;
namespace Esynctraining.Mp4Service.Tasks.Client
{
    public class DataTask
    {
        /// <summary>
        /// Unique task Id
        /// </summary>
        public Guid Id { set; get; }

        /// <summary>
        /// ScoId of recording
        /// </summary>
        public string ScoId { set; get; }

        /// <summary>
        /// ScoId of uploaded content result
        /// </summary>
        public string UploadScoId { set; get; }

        /// <summary>
        /// Customer grouping id
        /// </summary>
        public string LicenseId { set; get; }

        /// <summary>
        /// Last modified date of recording
        /// </summary>
        public DateTime Modified { set; get; }

        /// <summary>
        /// Duration of recording by AC
        /// </summary>
        public int Duration { set; get; }

        /// <summary>
        /// Current task status
        /// </summary>
        public TaskStatus Status { set; get; }

        /// <summary>
        /// ScoId of transcript
        /// </summary>
        public string TranscriptScoId { set; get; }

    }

    [Serializable]
    public enum TaskStatus
    {
        Pending,
        Queued,
        Recording,
        Recorded,
        Converting,
        Converted,
        Uploading,
        Uploaded,
        Transcripting,
        Transcripted,
        Completed
    }
}

