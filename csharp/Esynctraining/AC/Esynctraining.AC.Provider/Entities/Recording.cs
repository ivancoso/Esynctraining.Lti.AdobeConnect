using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    public class Recording
    {
        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        [XmlElement("date-modified")]
        public DateTime DateModified { get; set; }

        [XmlElement("url-path")]
        public string UrlPath { get; set; }

        [XmlElement("date-begin")]
        public DateTime BeginDate { get; set; }

        [XmlElement("date-end")]
        public DateTime EndDate { get; set; }

        [XmlAttribute("is-folder")]
        public bool IsFolder { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        [XmlAttribute("source-sco-id")]
        public string SourceScoId { get; set; }

        [XmlAttribute("folder-id")]
        public long FolderId { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("display-seq")]
        public int DisplaySequence { get; set; }

        [XmlAttribute("job-id")]
        public string JobId { get; set; }

        [XmlAttribute("account-id")]
        public string AccountId { get; set; }

        [XmlAttribute("job-status")]
        public string JobStatus { get; set; }

        [XmlAttribute("encoder-service-job-progress")]
        public int EncoderServiceJobProgress { get; set; }

        [XmlAttribute("no-of-downloads")]
        public int NoOfDownloads { get; set; }

        [XmlElement("filename")]
        public string Filename { get; set; }

        [XmlElement("encoder-service-job-params")]
        public string EncoderServiceJobParams { get; set; }

        [XmlElement("encoder-service-job-status")]
        public string EncoderServiceJobStatus { get; set; }

        [XmlElement("encoder-service-job-id")]
        public string EncoderServiceJobId { get; set; }

        [XmlElement("duration")]
        public string Duration { get; set; }

        public bool RecordingEdited { get; set; }
        public string RecordingEditedDuration { get; set; }
        public string AfRecordingDuration { get; set; }


        /// <summary>
        /// Returns RecordingEditedDuration if exists; 
        /// else AfRecordingDuration if exists;
        /// else Duration
        /// </summary>
        public TimeSpan? GetActualDuration()
        {
            //TimeSpan.Parse("01:55:56")
            //TimeSpan.Parse("02:12:29.353")
            string value = string.IsNullOrEmpty(RecordingEditedDuration)
                    ? (string.IsNullOrEmpty(AfRecordingDuration) ? Duration : AfRecordingDuration)
                    : RecordingEditedDuration;

            if (string.IsNullOrWhiteSpace(value))
                return null;
            try
            {
                // for recordings with >1 day duration: <duration>1d 07:52:30.243</duration> 
                return TimeSpan.Parse(value.Replace("d ", "."));
            }
            catch (FormatException ex)
            {
                throw new FormatException($"String '{value}' was not recognized as a valid TimeSpan.", ex);
            }
        }

    }

}
