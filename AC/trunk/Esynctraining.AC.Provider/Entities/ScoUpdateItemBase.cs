namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The sco update item base.
    /// </summary>
    [Serializable]
    public abstract class ScoUpdateItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoUpdateItemBase"/> class.
        /// </summary>
        protected ScoUpdateItemBase()
        {
            this.Type = ScoType.not_set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlElement("type")]
        public ScoType Type { get; set; }

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
        public string FolderId { get; set; }

        /// <summary>
        /// Gets or sets the url path.
        /// </summary>
        [XmlElement("url-path")]
        public string UrlPath { get; set; }

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
        /// Gets or sets the language.
        /// </summary>
        [XmlElement("lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the date begin.
        /// </summary>
        [XmlElement("date-begin")]
        public string DateBegin { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        [XmlElement("date-end")]
        public string DateEnd { get; set; }

        [XmlElement("sco-tag")]
        public string ScoTag { get; set; }
    }
}
