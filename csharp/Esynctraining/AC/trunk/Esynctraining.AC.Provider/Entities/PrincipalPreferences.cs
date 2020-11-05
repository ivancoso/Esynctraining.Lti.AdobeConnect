namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// Principal Preferences structure
    /// </summary>
    public class PrincipalPreferences
    {
        /// <summary>
        /// Gets or sets the ACL id.
        /// </summary>
        [XmlAttribute("acl-id")]
        public string AclId { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [XmlAttribute("lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the time zone id.
        /// </summary>
        [XmlAttribute("time-zone-id")]
        public string TimeZoneId { get; set; }

    }

}
