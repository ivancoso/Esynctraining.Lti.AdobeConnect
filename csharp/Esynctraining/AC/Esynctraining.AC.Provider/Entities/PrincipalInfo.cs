namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// PrincipalInfo structure
    /// </summary>
    [XmlRoot("results")]
    public class PrincipalInfo
    {
        /// <summary>
        /// Gets or sets the principal detail.
        /// </summary>
        [XmlElement("contact")]
        public Contact Contact { get; set; }

        /// <summary>
        /// Gets or sets the principal preferences.
        /// </summary>
        [XmlElement("contact")]
        public PrincipalPreferences PrincipalPreferences { get; set; }

        /// <summary>
        /// Gets or sets the principal.
        /// </summary>
        [XmlElement("principal")]
        public PrincipalDetail Principal { get; set; }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        [XmlElement("manager")]
        public Principal Manager { get; set; }

    }

}
