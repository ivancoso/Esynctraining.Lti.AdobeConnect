namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;


    /// <summary>
        /// SCO Content.
        /// </summary>
        [Serializable]
    public class ScoContent
    {
        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the source SCO id.
        /// </summary>
        [XmlAttribute("source-sco-id")]
        public string SourceScoId { get; set; }

        /// <summary>
        /// Gets or sets the folder id.
        /// </summary>
        [XmlAttribute("folder-id")]
        public long FolderId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        [XmlAttribute("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the display sequence.
        /// </summary>
        [XmlAttribute("display-seq")]
        public int DisplaySequence { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [XmlAttribute("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is folder.
        /// </summary>
        [XmlAttribute("is-folder")]
        public bool IsFolder { get; set; }

        [XmlElement("byte-count")]
        public int ByteCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is folder.
        /// </summary>
        [XmlAttribute("is-seminar")]
        public bool IsSeminar { get; set; }

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
        /// Gets or sets the begin date local.
        /// </summary>
        [XmlElement("date-begin")]
        public DateTime BeginDateLocal { get; set; }

        /// <summary>
        /// Gets or sets the end date local.
        /// </summary>
        [XmlElement("date-end")]
        public DateTime EndDateLocal { get; set; }

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
    }
}
