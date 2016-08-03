namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// Principal detail structure
    /// </summary>
    [XmlRoot("principal")]
    public class PrincipalDetail : Principal
    {
        /// <summary>
        /// Gets or sets the external login.
        /// </summary>
        [XmlElement("ext-login")]
        public string ExternalLogin { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [XmlElement("first-name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [XmlElement("last-name")]
        public string LastName { get; set; }

    }

}
