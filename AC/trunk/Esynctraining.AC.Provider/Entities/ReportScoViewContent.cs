using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    public class ReportScoViewContent
    {
        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is folder.
        /// </summary>
        [XmlAttribute("is-folder")]
        public bool IsFolder { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("views")]
        public int Views { get; set; }

        [XmlAttribute("last-viewed-date")]
        public DateTime LastViewedDate { get; set; }

    }

}