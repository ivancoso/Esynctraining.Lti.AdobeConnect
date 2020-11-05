using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    [XmlRoot("recording-job")]
    public class RecordingJob
    {
        [XmlAttribute("folder-id")]
        public string FolderId { get; set; }

        [XmlAttribute("duration")]
        public int Duration { get; set; }

        [XmlAttribute("account-id")]
        public string AccountId { get; set; }

        [XmlAttribute("credit-minute")]
        public int CreditMinute { get; set; }

        [XmlAttribute("debit-minute")]
        public int DebitMinute { get; set; }

        [XmlAttribute("encoder-service-job-progress")]
        public int EncoderServiceJobProgress { get; set; }

        [XmlAttribute("job-id")]
        public string JobId { get; set; }

        [XmlAttribute("job-status")]
        public string JobStatus { get; set; }

        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        [XmlAttribute("retry-number")]
        public int RetryNumber { get; set; }

        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        [XmlAttribute("source-sco-id")]
        public string SourceScoId { get; set; }

        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        [XmlElement("date-modified")]
        public DateTime DateModified { get; set; }

        [XmlElement("encoder-service-job-id")]
        public string EncoderServiceJobId { get; set; }

        [XmlElement("encoder-service-job-params")]
        public string EncoderServiceJobParams { get; set; }

        [XmlElement("encoder-service-job-status")]
        public string EncoderServiceJobStatus { get; set; }

        [XmlElement("rec-service-host-name")]
        public string RecServiceHostName { get; set; }

        [XmlElement("rec-service-ip-address")]
        public string RecServiceIpAddress { get; set; }

    }

}
