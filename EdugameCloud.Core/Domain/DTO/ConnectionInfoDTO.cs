namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The connection info dto.
    /// </summary>
    [DataContract]
    public class ConnectionInfoDTO
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
