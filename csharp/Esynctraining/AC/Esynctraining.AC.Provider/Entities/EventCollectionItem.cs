namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Event collection item information 
    /// </summary>
    public class EventCollectionItem
    {
        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

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
        /// Gets or sets the duration.
        /// </summary>
        [XmlElement("duration")]
        public TimeSpan Duration { get; set; }
    }
}
