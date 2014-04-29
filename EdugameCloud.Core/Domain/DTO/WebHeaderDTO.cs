namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The web header DTO.
    /// </summary>
    [DataContract]
    public class WebHeaderDTO
    {
        #region Public Properties

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
