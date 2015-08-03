namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// User structure
    /// http://help.adobe.com/en_US/connect/9.0/webservices/WS5b3ccc516d4fbf351e63e3d11a171ddf77-7f2d_SP1.html
    /// </summary>
    [Serializable]
    [XmlRoot("row")]
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.Type = PrincipalTypes.user;
        }

        /// <summary>
        /// Gets or sets the Principal id.
        /// </summary>
        [XmlAttribute("principal-id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the type (user or guest)
        /// </summary>
        [XmlElement]
        [XmlAttribute("type")]
        public PrincipalTypes Type { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("manager")]
        public string Manager { get; set; }

    }

}
