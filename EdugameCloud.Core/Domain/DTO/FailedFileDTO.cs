namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The failed file DTO.
    /// </summary>
    [DataContract]
    public class FailedFileDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [DataMember]
        public string error { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [DataMember]
        public string fileName { get; set; }

        #endregion
    }
}