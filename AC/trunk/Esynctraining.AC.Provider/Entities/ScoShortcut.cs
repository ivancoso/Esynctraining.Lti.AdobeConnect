namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// SCO Shortcut structure.
    /// </summary>
    [Serializable]
    public class ScoShortcut
    {
        /// <summary>
        /// Gets or sets the tree id.
        /// </summary>
        [XmlAttribute("tree-id")]
        public int TreeId { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [XmlAttribute("sco-id")]
        public string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        [XmlElement("domain-name")]
        public string DomainName { get; set; }
    }
}
