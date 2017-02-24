namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// SCO Info.
    /// </summary>
    [XmlRoot("sco")]
    public class ScoInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoInfo"/> class.
        /// </summary>
        public ScoInfo()
        {
            this.Type = ScoType.not_set;
        }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        [XmlAttribute("account-id")]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the folder id.
        /// </summary>
        [XmlAttribute("folder-id")]
        public string FolderId { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the source SCO id.
        /// </summary>
        [XmlAttribute("source-sco-id")]
        public string SourceScoId { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [XmlAttribute("lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlElement]
        public ScoType Type { get; set; }

        /// <summary>
        /// Gets or sets the external url.
        /// </summary>
        [XmlElement("external-url")]
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the external url.
        /// </summary>
        [XmlElement("max-score")]
        public int MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the begin date.
        /// </summary>
        [XmlElement("date-begin")]
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [XmlElement("date-end")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [XmlElement("date-created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [XmlElement("date-modified")]
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the url path.
        /// </summary>
        [XmlElement("url-path")]
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets the passing score.
        /// </summary>
        [XmlElement("passing-score")]
        public int PassingScore { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [XmlElement("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the section count.
        /// </summary>
        [XmlElement("section-count")]
        public int SectionCount { get; set; }

        /// <summary>
        /// For Meeting sco id - audio profile id
        /// </summary>
        [XmlElement("telephony-profile")]
        public string TelephonyProfile { get; set; }

        /// <summary>
        /// For Virtual classroom - Classroom ID on UI
        /// </summary>
        [XmlElement("sco-tag")]
        public string ScoTag { get; set; }

        /// <summary>
        /// For recordings/meetings that require passcode.
        /// </summary>
        [XmlElement("meeting-passcode")]
        public string MeetingPasscode { get; set; }

        public ScoInfo SourceSco { get; set; }
    }
}
