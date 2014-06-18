namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The tag request DTO.
    /// </summary>
    [DataContract]
    public class TagRequestDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        [DataMember]
        public string tag { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        [DataMember]
        public Int64 time { get; set; }

        #endregion
    }
}