namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The login with history DTO.
    /// </summary>
    [DataContract]
    public class LoginWithHistoryDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        [DataMember]
        public string passwordHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether send splash screen.
        /// </summary>
        [DataMember]
        public bool sendSplashScreen { get; set; }

        /// <summary>
        /// Gets or sets the from IP.
        /// </summary>
        [DataMember]
        public string fromIP { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        [DataMember]
        public string application { get; set; }

        /// <summary>
        /// Gets or sets the lms user parameters id.
        /// </summary>
        [DataMember]
        public int? lmsUserParametersId { get; set; }

        #endregion
    }
}