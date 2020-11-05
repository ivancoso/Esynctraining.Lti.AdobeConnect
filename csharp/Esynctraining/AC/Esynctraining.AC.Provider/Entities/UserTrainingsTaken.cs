using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    [XmlRoot("row")]
    public class UserTrainingsTaken
    {
        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("transcript-id")]
        public string TranscriptId { get; set; }

        [XmlAttribute("max-retries")]
        public string MaxRetries { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("certificate")]
        public string Certificate { get; set; }

        [XmlAttribute("permission-id")]
        public PermissionId PermissionId { get; set; }

        [XmlAttribute("score")]
        public int Score { get; set; }

        [XmlAttribute("max-score")]
        public int MaxScore { get; set; }

        [XmlAttribute("attempts")]
        public int Attempts { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("url-path")]
        public string UrlPath { get; set; }

        [XmlElement("sco-tag")]
        public string ScoTag { get; set; }

        [XmlElement("date-taken")]
        public DateTime DateTaken { get; set; }

        [XmlElement("from-curriculum")]
        public bool FromCurriculum { get; set; }

    }
}