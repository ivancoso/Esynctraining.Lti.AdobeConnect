using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    [Serializable]
    public class Recording : ScoContent
    {
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
    }
}
