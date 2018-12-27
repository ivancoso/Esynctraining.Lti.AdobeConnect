namespace EdugameCloud.Lti.DTO
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// The LMS user settings DTO.
    /// </summary>
    [DataContract]
    public class LmsUserSettingsDTO
    {
        /// <summary>
        /// Gets or sets the AC connection mode.
        /// </summary>
        [Required]
        [DataMember]
        public int AcConnectionMode { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public string Password { get; set; }

    }

}
