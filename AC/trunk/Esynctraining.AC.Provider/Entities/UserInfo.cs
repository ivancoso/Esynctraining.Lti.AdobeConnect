namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// UserInfo structure
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [XmlAttribute("user-id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }
    }
}
