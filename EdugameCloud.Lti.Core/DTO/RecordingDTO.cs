﻿using Esynctraining.AC.Provider.Entities;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class RecordingDTO
    {
        public RecordingDTO(Recording recording, string accountUrl)
        {
            id = recording.ScoId;
            name = recording.Name;
            begin_date = recording.BeginDate.ToString("MM/dd/yy h:mm:ss tt");
            duration = GetDurationWithoutMilliseconds(recording.Duration);
            url = GenerateJoinLink(recording.UrlPath);
            status = GetRecordingStatus(recording.JobStatus);
            is_mp4 = recording.Icon == "mp4-archive";
            job_id = string.IsNullOrEmpty(recording.JobId) ? string.Empty : recording.JobId;
            download_url = this.is_mp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : string.Empty;
        }

        public RecordingDTO() { }


        /// <summary>
        /// Gets or sets a recording url for download. Available only for mp4.
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
        
        [DataMember]
        public bool is_mp4 { get; set; }
        
        [DataMember]
        public string begin_date { get; set; }

        //[DataMember]
        //public string description { get; set; }
        
        [DataMember]
        public string duration { get; set; }

        //[DataMember]
        //public string end_date { get; set; }
        
        [DataMember]
        public string id { get; set; }
        
        [DataMember]
        public string name { get; set; }
        
        [DataMember]
        public string url { get; set; }
        
        [DataMember]
        public bool is_public { get; set; }

        //[DataMember]
        //public string password { get; set; }

        #region methods

        private static string GetRecordingStatus(string jobStatus)
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

        private static string GenerateJoinLink(string recordingUrlPath)
        {
            return "/Lti/Recording/Join/" + recordingUrlPath.Trim("/".ToCharArray());
        }

        private static string GenerateDownloadLink(string accountUrl, string recordingPath, string recordingName)
        {
            return accountUrl + recordingPath + "output/ " + recordingName + ".zip?download=zip";
        }

        private static string GetDurationWithoutMilliseconds(string date)
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