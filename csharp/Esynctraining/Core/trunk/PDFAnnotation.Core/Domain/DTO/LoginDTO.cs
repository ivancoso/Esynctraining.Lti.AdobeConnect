namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The login dto.
    /// </summary>
    [DataContract]
    public class LoginDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether system should remember user.
        /// </summary>
        [DataMember]
        public bool rememberMe { get; set; }

        #endregion
    }
}