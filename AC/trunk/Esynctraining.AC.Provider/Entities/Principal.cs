namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// The principal.
    /// </summary>
    [XmlRoot("principal")]
    public class Principal
    {
        [XmlAttribute("account-id")]
        public string AccountId { get; set; }

        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        [XmlAttribute("has-children")]
        public bool HasChildren { get; set; }

        [XmlAttribute("is-hidden")]
        public bool IsHidden { get; set; }

        [XmlAttribute("is-primary")]
        public bool IsPrimary { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("login")]
        public string Login { get; set; }

        [XmlElement("display-uid")]
        public string DisplayId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        //[XmlElement("first-name")]
        //public string FirstName { get; set; }

        //[XmlElement("last-name")]
        //public string LastName { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }


        /// <summary>
        /// Actual for groups only.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

    }

}
