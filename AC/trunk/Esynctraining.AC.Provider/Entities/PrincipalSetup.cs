namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// PrincipalSetup structure
    /// </summary>
    public class PrincipalSetup
    {
        /// <summary>
        /// The type of principal. Use only when creating a new principal
        /// </summary>
        [XmlElement("type")]
        public PrincipalTypes Type { get; set; }

        /// <summary>
        /// The principal’s new login name, usually
        /// the principal’s e-mail address. Must be
        /// unique on the server. Required to create
        /// or update a user. Do not use with groups.
        /// </summary>
        [XmlElement("login")]
        public string Login { get; set; }

        /// <summary>
        /// The new group’s name. Use only when
        /// creating a new group. Required to create
        /// a group.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// The user’s new first name. Use only with
        /// users, not with groups. Required to create a user
        /// </summary>
        [XmlElement("first-name")]
        public string FirstName { get; set; }

        /// <summary>
        /// The new last name to assign to the user.
        /// Required to create a user. Do not use with groups.
        /// </summary>
        [XmlElement("last-name")]
        public string LastName { get; set; }
        
        /// <summary>
        /// The user’s e-mail address. Can be
        /// different from the login. Be sure to
        /// specify a value if you use sendemail=true.
        /// </summary>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// The new user’s password. Use only when creating a new user.
        /// </summary>
        [XmlElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// The new group’s description. Use only when creating a new group.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the principal has children. If the
        /// principal is a group, use 1 or true. If the
        /// principal is a user, use 0 or false.
        /// </summary>
        [XmlElement("has-children")]
        public bool HasChildren { get; set; }

        /// <summary>
        /// The ID of the principal that has
        /// information you want to update. Required
        /// to update a user or group, but do not use
        /// to create either.
        /// </summary>
        [XmlElement("principal-id")]
        public string PrincipalId { get; set; }

        /// <summary>
        /// A flag indicating whether the server
        /// should send an e-mail to the principal with
        /// account and login information.
        /// </summary>
        [XmlElement("send-email")]
        public bool SendEmail { get; set; }
    }
}
