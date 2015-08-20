using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public sealed class ConnectionTestDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [DataMember]
        public string domain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable proxy tool mode.
        /// </summary>
        [DataMember]
        public bool enableProxyToolMode { get; set; }

        /// <summary>
        ///     Gets or sets the info.
        /// </summary>
        [DataMember]
        public string login { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        #endregion
    }

}
