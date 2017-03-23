using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Esynctraining.AdobeConnect.Api.MeetingRecording.Dto
{
    // TODO: GenerateJoinLink!!!
    [DataContract]
    public class RecordingDto : IRecordingDto
    {
        public RecordingDto(Recording recording, string accountUrl, TimeZoneInfo timezone)
        {
            Id = recording.ScoId;
            Name = recording.Name;
            // TRICK: cleanup not to output empty strings in json
            Summary = string.IsNullOrWhiteSpace(recording.Description) ? null: recording.Description;
            BeginAt = (long)recording.BeginDate.ConvertToUnixTimestamp() + (long)GetTimezoneShift(timezone, recording.BeginDate);
            Duration = GetDurationWithoutMilliseconds(recording.Duration);
            Url = GenerateJoinLink(recording.UrlPath);
            status = GetRecordingStatus(recording.JobStatus);
            IsAdobeMp4 = recording.Icon == "mp4-archive";
            downloadUrl = this.IsAdobeMp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : null;
        }

        public RecordingDto(ScoContent recording, string accountUrl, bool isPublic, TimeZoneInfo timezone)
        {
            Id = recording.ScoId;
            Name = recording.Name;
            // TRICK: cleanup not to output empty strings in json
            Summary = string.IsNullOrWhiteSpace(recording.Description) ? null : recording.Description;
            BeginAt = (long)recording.BeginDate.ConvertToUnixTimestamp();
            Duration = ConvertSecondsToTimeFormat(recording.Duration) + (long)GetTimezoneShift(timezone, recording.BeginDate);
            Url = GenerateJoinLink(recording.UrlPath);
            IsAdobeMp4 = recording.Icon == "mp4-archive";
            downloadUrl = this.IsAdobeMp4 ? GenerateDownloadLink(accountUrl, recording.UrlPath, recording.Name) : null;
            IsPublic = isPublic;
        }

        public RecordingDto() { }


        /// <summary>
        /// Gets or sets a recording url for download. Available only for mp4.
        /// </summary>
        [DataMember]
        public string downloadUrl { get; set; }

        /// <summary>
        /// Gets or sets a recording status.
        /// </summary>
        [DataMember]
        public string status { get; set; }

        // AC disabled creation of mp4 on their side, but mp4's are still there for a lot of users (as they used their api too). 
        // That flag indicates that this rec was converted to mp4 on their side
        [DataMember(Name = "isMp4")]
        public bool IsAdobeMp4 { get; set; }

        [Required]
        [DataMember]
        public long BeginAt { get; set; }

        [DataMember]
        public string Duration { get; set; }

        [Required]
        [DataMember]
        public string Id { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [Required]
        [DataMember]
        public string Url { get; set; }

        [Required]
        [DataMember]
        public bool IsPublic { get; set; }

        [DataMember]
        public bool Published { get; set; }

        #region methods

        private static string GetRecordingStatus(string jobStatus)
        {
            if (string.IsNullOrEmpty(jobStatus))
            {
                return null;
            }

            string recordingStatus = null;

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
            return recordingUrlPath.Trim('/');
        }

        private static string GenerateDownloadLink(string accountUrl, string recordingPath, string recordingName)
        {
            return accountUrl + recordingPath + "output/ " + recordingName + ".zip?download=zip";
        }

        private static string GetDurationWithoutMilliseconds(string date)
        {
            var index = date.IndexOf('.');
            if (index > 0)
            {
                return date.Substring(0, index);
            }
            return date;
        }

        private static string ConvertSecondsToTimeFormat(int seconds)
        {
            var t = TimeSpan.FromSeconds(seconds);
            return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }

        private static double GetTimezoneShift(TimeZoneInfo timezone, DateTime value)
        {
            if (timezone != null)
            {
                var offset = timezone.GetUtcOffset(value).TotalMilliseconds;
                return offset;
            }

            return 0;
        }

        #endregion

    }
}
