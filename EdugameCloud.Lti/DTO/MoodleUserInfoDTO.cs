namespace EdugameCloud.Lti.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The moodle user info DTO.
    /// </summary>
    [DataContract]
    public class MoodleUserInfoDTO
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public virtual string name { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public virtual string password { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [DataMember]
        public virtual string domain { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [DataMember]
        public virtual string provider { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        [DataMember]
        public virtual string token { get; set; }

        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        [DataMember]
        public virtual string courseId { get; set; }
    }
}
