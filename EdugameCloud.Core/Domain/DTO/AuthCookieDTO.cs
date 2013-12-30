namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The authentication cookie DTO.
    /// </summary>
    [DataContract]
    public class AuthCookieDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date time ticks.
        /// </summary>
        [DataMember]
        public double dateTimeTicks { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DataMember]
        public string value { get; set; }

        #endregion
    }
}