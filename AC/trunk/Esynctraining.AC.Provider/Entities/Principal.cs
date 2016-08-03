namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// The principal.
    /// </summary>
    [XmlRoot("principal")]
    public class Principal
    {
        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        [XmlAttribute("account-id")]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has children.
        /// </summary>
        [XmlAttribute("has-children")]
        public bool HasChildren { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is hidden.
        /// </summary>
        [XmlAttribute("is-hidden")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is primary.
        /// </summary>
        [XmlAttribute("is-primary")]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether type.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the display id.
        /// </summary>
        [XmlElement("display-uid")]
        public string DisplayId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("first-name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [XmlElement("last-name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

    }

}
