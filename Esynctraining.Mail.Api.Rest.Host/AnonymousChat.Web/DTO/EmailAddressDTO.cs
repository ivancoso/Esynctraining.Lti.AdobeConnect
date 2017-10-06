namespace AnonymousChat.Web.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The email address DTO.
    /// </summary>
    [DataContract]
    public class EmailAddressDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        #endregion
    }
}