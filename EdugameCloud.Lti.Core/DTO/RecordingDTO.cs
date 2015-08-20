using System;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The recording DTO.
    /// </summary>
    [DataContract]
    public sealed class RecordingDTO
    {
        public RecordingDTO(RecordingJob recordingJob)
        {
            this.id = recordingJob.ScoId;
            this.begin_date = recordingJob.DateCreated.ToString("MM/dd/yy h:mm:ss tt");
            this.status = this.GetRecordingStatus(recordingJob.JobStatus);
            this.is_mp4 = true;
        }
        public RecordingDTO(Recording recording, string accountUrl)
        {
            this.id = recording.ScoId;
            this.name = recording.Name;
            this.begin_date = recording.BeginDate.ToString("MM/dd/yy h:mm:ss tt");
            this.duration = this.GetDurationWithoutMilliseconds(recording.Duration);
            this.url = this.GenerateJoinLink(recording.UrlPath);
            this.status = this.GetRecordingStatus(recording.JobStatus);
            this.is_mp4 = recording.Icon == "mp4-archive";
            this.job_id = string.IsNullOrEmpty(recording.JobId) ? string.Empty : recording.JobId;
            this.download_url = this.is_mp4 ? this.GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : string.Empty;
        }
        public RecordingDTO()
        {
            
        }

        /// <summary>
        /// Gets or sets a recording url for download.Available only for mp4.
        /// </summary>
        [DataMember]
        public string download_url { get; set; }

        /// <summary>
        /// Gets or sets a recording job id.
        /// </summary>
        [DataMember]
        public string job_id { get; set; }

        /// <summary>
        /// Gets or sets a recording status.
        /// </summary>
        [DataMember]
        public string status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_mp4.
        /// </summary>
        [DataMember]
        public bool is_mp4 { get; set; }

        /// <summary>
        /// Gets or sets the begin date.
        /// </summary>
        [DataMember]
        public string begin_date { get; set; }

        //[DataMember]
        //public string description { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [DataMember]
        public string duration { get; set; }

        //[DataMember]
        //public string end_date { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [DataMember]
        public string url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_public.
        /// </summary>
        [DataMember]
        public bool is_public { get; set; }

        //[DataMember]
        //public string password { get; set; }
   
        #region methods

        private string GetRecordingStatus(string jobStatus)
        {
            if (string.IsNullOrEmpty(jobStatus))
            {
                return string.Empty;
            }

            var recordingStatus = string.Empty;

            switch (jobStatus)
            {
                case "job-pending":
                    recordingStatus = "in_progress";
                    break;
                case "job-completed":
                    recordingStatus = "completed";
                    break;
                case "job-queued":
                    recordingStatus = "job-queued";
                    break;
                case "job-error":
                    recordingStatus = "job-error";
                    break;
                case "job-downloading":
                    recordingStatus = "job-downloading";
                    break;
            }

            return recordingStatus;
        }
        private string GenerateJoinLink(string recordingUrlPath)
        {
            return "/Lti/Recording/Join/" + recordingUrlPath.Trim("/".ToCharArray());
        }
        private string GenerateDownloadLink(string accountUrl, string recordingPath, string recordingName)
        {
            return accountUrl + recordingPath + "output/ " + recordingName + ".zip?download=zip";
        }
        private string GetDurationWithoutMilliseconds(string date)
        {
            var index = date.IndexOf(".");
            if (index > 0)
            {
                return date.Substring(0, index);
            }

            return date;
        }

        #endregion


    }

}