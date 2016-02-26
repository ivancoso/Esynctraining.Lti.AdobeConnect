using System;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using EdugameCloud.Core.Extensions;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class Mp4ServiceStatusDto
    {
        private readonly string _baseFileAccessUrl;


        [DataMember]
        public string mp4_sco_id { get; set; }

        [DataMember]
        public string cc_sco_id { get; set; }
        
        [DataMember]
        public string status { get; set; }


        [DataMember]
        public string mp4_url
        {
            get
            {
                if (string.IsNullOrWhiteSpace(mp4_sco_id))
                    return null;
                // TODO: config!!
                //return string.Format("https://dev.esynctraining.com/contentApi/mp4/{0}", mp4_sco_id);
                return string.Format("https://dev.edugamecloud.com/lti/mp4/file/{0}", mp4_sco_id);
            }
            set
            {
            }
        }

        [DataMember]
        public string cc_ulr
        {
            get
            {
                if (string.IsNullOrWhiteSpace(cc_sco_id))
                    return null;
                // TODO: config!!
                return string.Format("https://dev.esynctraining.com/contentApi/subtitle/{0}", cc_sco_id);
            }
            set
            {
            }
        }

        public string lmsProviderName { get; set; }

        public Mp4ServiceStatusDto()
        {
            //if (string.IsNullOrWhiteSpace(baseFileAccessUrl))
            //    throw new ArgumentException("baseFileAccessUrl should have have", "baseFileAccessUrl");

            //Uri result;
            //if (!Uri.TryCreate(baseFileAccessUrl, UriKind.Absolute, out result))
            //    throw new ArgumentException("baseFileAccessUrl should have Absolute Url value", "baseFileAccessUrl");

            //_baseFileAccessUrl = baseFileAccessUrl;
        }

    }


    [DataContract]
    public sealed class RecordingDTO
    {
        public RecordingDTO(Recording recording, string accountUrl)
        {
            id = recording.ScoId;
            name = recording.Name;
            begin_date = recording.BeginDate.ToString("MM/dd/yy h:mm:ss tt");
            beginAt = (long)recording.BeginDate.ConvertToUnixTimestamp();
            duration = GetDurationWithoutMilliseconds(recording.Duration);
            url = GenerateJoinLink(recording.UrlPath);
            status = GetRecordingStatus(recording.JobStatus);
            is_mp4 = recording.Icon == "mp4-archive";
            download_url = this.is_mp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : string.Empty;
        }

        public RecordingDTO(ScoContent recording, string accountUrl, bool isPublic)
        {
            id = recording.ScoId;
            name = recording.Name;
            begin_date = recording.BeginDate.ToString("MM/dd/yy h:mm:ss tt");
            beginAt = (long)recording.BeginDate.ConvertToUnixTimestamp();
            duration = ConvertSecondsToTimeFormat(recording.Duration);
            url = GenerateJoinLink(recording.UrlPath);
            is_mp4 = recording.Icon == "mp4-archive";
            download_url = this.is_mp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : string.Empty;
            is_public = isPublic;
        }

        public RecordingDTO() { }


        [DataMember]
        public Mp4ServiceStatusDto mp4 { get; set; }

        /// <summary>
        /// Gets or sets a recording url for download. Available only for mp4.
        /// </summary>
        [DataMember]
        public string download_url { get; set; }
        
        /// <summary>
        /// Gets or sets a recording status.
        /// </summary>
        [DataMember]
        public string status { get; set; }
        
        [DataMember]
        public bool is_mp4 { get; set; }
        
        [DataMember]
        [ScriptIgnore]
        public string begin_date { get; set; }

        [DataMember]
        public long beginAt { get; set; }

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

        [DataMember]
        public bool published { get; set; }
        
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

        private static string ConvertSecondsToTimeFormat(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);

            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds);
        }
        #endregion
        
    }

}