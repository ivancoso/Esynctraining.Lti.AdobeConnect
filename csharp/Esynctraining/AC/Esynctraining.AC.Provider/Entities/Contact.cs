namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// The contact.
    /// </summary>
    public class Contact
    {
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
