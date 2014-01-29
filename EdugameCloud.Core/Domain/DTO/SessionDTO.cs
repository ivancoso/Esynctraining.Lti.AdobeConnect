namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The authentication cookie DTO.
    /// </summary>
    [DataContract]
    public class SessionDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date time ticks.
        /// </summary>
        [DataMember]
        public DateTime expiration { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DataMember]
        public string session { get; set; }

        #endregion
    }
}