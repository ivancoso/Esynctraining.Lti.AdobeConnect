namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Event information 
    /// </summary>
    [Serializable]
    public class EventInfo
    {
        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the tree id.
        /// </summary>
        [XmlAttribute("tree-id")]
        public long TreeId { get; set; }

        /// <summary>
        /// Gets or sets the permission id.
        /// </summary>
        [XmlAttribute("permission-id")]
        public PermissionId PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        [XmlElement("domain-name")]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the date begin.
        /// </summary>
        [XmlElement("date-begin")]
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        [XmlElement("date-end")]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Gets or sets the url path.
        /// </summary>
        [XmlElement("url-path")]
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether expired.
        /// </summary>
        [XmlElement("expired")]
        public bool Expired { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [XmlElement("duration")]
        public TimeSpan Duration { get; set; }   
    }
}
