using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    /// <summary>
    /// The connection info DTO.
    /// </summary>
    [DataContract]
    public sealed class ConnectionInfoDTO
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [DataMember]
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        [DataMember]
        public string info { get; set; }
    }

}
