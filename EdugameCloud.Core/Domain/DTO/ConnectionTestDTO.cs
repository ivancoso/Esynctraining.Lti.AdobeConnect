// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionTestDTO.cs" company="">
//   
// </copyright>
// <summary>
//   The connection test dto.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     The connection test DTO.
    /// </summary>
    [DataContract]
    public class ConnectionTestDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [DataMember]
        public string domain { get; set; }

        /// <summary>
        ///     Gets or sets the info.
        /// </summary>
        [DataMember]
        public string login { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        /// Gets or sets the proxy tool mode.
        /// </summary>
        [DataMember]
        public bool enableProxyToolMode { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public string type { get; set; }

        #endregion
    }
}