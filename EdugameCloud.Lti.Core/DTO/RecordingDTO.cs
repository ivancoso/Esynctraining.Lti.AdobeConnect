//using System;
//using System.Runtime.Serialization;
//using System.Web.Script.Serialization;
//using Esynctraining.Core.Extensions;
//using Esynctraining.AC.Provider.Entities;
//using Esynctraining.AdobeConnect;

//namespace EdugameCloud.Lti.Core.DTO
//{
//    [DataContract]
//    public class RecordingDTO : IRecordingDto
//    {
//        public RecordingDTO(Recording recording, string accountUrl, TimeZoneInfo timezone)
//        {
//            Id = recording.ScoId;
//            name = recording.Name;
//            summary = recording.Description;
//            begin_date = recording.BeginDate.ToString("MM/dd/yy h:mm:ss tt");
//            BeginAt = (long)recording.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timezone, recording.BeginDate);
//            duration = GetDurationWithoutMilliseconds(recording.GetDuration());
//            url = GenerateJoinLink(recording.UrlPath);
//            status = GetRecordingStatus(recording.JobStatus);
//            is_mp4 = recording.Icon == "mp4-archive";
//            download_url = this.is_mp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : null;
//        }

//        public RecordingDTO(ScoContent recording, string accountUrl, bool isPublic, TimeZoneInfo timezone)
//        {
//            Id = recording.ScoId;
//            name = recording.Name;
//            summary = recording.Description;
//            begin_date = recording.BeginDate.ToString("MM/dd/yy h:mm:ss tt");
//            BeginAt = (long)recording.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timezone, recording.BeginDate);
//            duration = ConvertSecondsToTimeFormat(recording.Duration);
//            url = GenerateJoinLink(recording.UrlPath);
//            is_mp4 = recording.Icon == "mp4-archive";
//            download_url = this.is_mp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : null;
//            IsPublic = isPublic;
//        }

//        public RecordingDTO() { }


//        /// <summary>
//        /// Gets or sets a recording url for download. Available only for mp4.
//        /// </summary>
//        [DataMember]
//        public string download_url { get; set; }

//        /// <summary>
//        /// Gets or sets a recording status.
//        /// </summary>
//        [DataMember]
//        public string status { get; set; }

//        [DataMember]
//        public bool is_mp4 { get; set; }

//        [DataMember]
//        [ScriptIgnore]
//        public string begin_date { get; set; }

//        [DataMember(Name = "beginAt")]
//        public long BeginAt { get; set; }

//        //[DataMember]
//        //public string description { get; set; }

//        [DataMember]
//        public string duration { get; set; }

//        //[DataMember]
//        //public string end_date { get; set; }

//        [DataMember(Name = "id")]
//        public string Id { get; set; }

//        [DataMember]
//        public string name { get; set; }

//        [DataMember]
//        public string summary { get; set; }

//        [DataMember]
//        public string url { get; set; }

//        [DataMember(Name = "is_public")]
//        public bool IsPublic { get; set; }

//        //[DataMember]
//        //public string password { get; set; }

//        [DataMember(Name = "published")]
//        public bool Published { get; set; }

//        #region methods

//        private static string GetRecordingStatus(string jobStatus)
//        {
//            if (string.IsNullOrEmpty(jobStatus))
//            {
//                return null;
//            }

//            string recordingStatus = null;

//            switch (jobStatus)
//            {
//                case "job-pending":
//                    recordingStatus = "in_progress";
//                    break;
//                case "job-completed":
//                    recordingStatus = "completed";
//                    break;
//                case "job-queued":
//                    recordingStatus = "job-queued";
//                    break;
//                case "job-error":
//                    recordingStatus = "job-error";
//                    break;
//                case "job-downloading":
//                    recordingStatus = "job-downloading";
//                    break;
//            }

//            return recordingStatus;
//        }

//        private static string GenerateJoinLink(string recordingUrlPath)
//        {
//            //запилить сеттинг как в ССО для адреса - тут отдавать только urlPath
//            return "recordings/join/" + recordingUrlPath.Trim("/".ToCharArray());
//        }

//        private static string GenerateDownloadLink(string accountUrl, string recordingPath, string recordingName)
//        {
//            return accountUrl + recordingPath + "output/ " + recordingName + ".zip?download=zip";
//        }

//        private static string GetDurationWithoutMilliseconds(string date)
//        {
//            var index = date.IndexOf(".");
//            if (index > 0)
//            {
//                return date.Substring(0, index);
//            }

//            return date;
//        }

//        private static string ConvertSecondsToTimeFormat(int seconds)
//        {
//            TimeSpan t = TimeSpan.FromSeconds(seconds);

//            return string.Format("{0:D2}:{1:D2}:{2:D2}",
//                t.Hours,
//                t.Minutes,
//                t.Seconds);
//        }

//        private static double GetTimezoneShift(TimeZoneInfo timezone, DateTime value)
//        {
//            if (timezone != null)
//            {
//                var offset = timezone.GetUtcOffset(value).TotalMilliseconds;
//                return offset;
//            }

//            return 0;
//        }

//        #endregion

//    }

//    public static class ACExtensions //todo: move to lib
//    {
//        //returns milliseconds
//        //public static long GetDuration(this Recording recording)
//        //{
//        //    return
//        //        GetMillisecondsFromTimeFormat(string.IsNullOrEmpty(recording.RecordingEditedDuration)
//        //            ? (string.IsNullOrEmpty(recording.AfRecordingDuration) ? recording.Duration : recording.AfRecordingDuration)
//        //            : recording.RecordingEditedDuration);
//        //}

//        public static string GetDuration(this Recording recording)
//        {
//            return
//                string.IsNullOrEmpty(recording.RecordingEditedDuration)
//                    ? (string.IsNullOrEmpty(recording.AfRecordingDuration) ? recording.Duration : recording.AfRecordingDuration)
//                    : recording.RecordingEditedDuration;
//        }

//        //private static long GetMillisecondsFromTimeFormat(string timeFormat)
//        //{
//        //    if (string.IsNullOrEmpty(timeFormat))
//        //        throw new ArgumentNullException(nameof(timeFormat));

//        //    TimeSpan ts = TimeSpan.Parse(timeFormat);
//        //    double totalMilliseconds = ts.TotalMilliseconds;
//        //    return (long)totalMilliseconds;
//        //}

//    }
    
//}