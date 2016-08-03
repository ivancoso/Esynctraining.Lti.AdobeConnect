namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// PermissionInfo structure
    /// </summary>
    public class PermissionInfo
    {
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
        /// Gets or sets a value indicating whether is primary.
        /// </summary>
        [XmlAttribute("is-primary")]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the permission id.
        /// </summary>
        [XmlAttribute("permission-id")]
        public PermissionId PermissionId { get; set; }

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
        /// Gets or sets the description.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

    }

}
