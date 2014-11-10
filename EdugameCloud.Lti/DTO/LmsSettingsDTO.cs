﻿
namespace EdugameCloud.Lti.DTO
{
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
        [DataMember]
        public int acConnectionMode { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider name.
        /// </summary>
        [DataMember]
        public string lmsProviderName { get; set; }
    }
}
