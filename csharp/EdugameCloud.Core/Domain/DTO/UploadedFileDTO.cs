namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The File DTO.
    /// </summary>
    [DataContract]
    public class UploadedFileDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string contentType { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        [DataMember(IsRequired = false)]
        public byte[] content { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(IsRequired = false)]
        public Guid fileId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string fileName { get; set; }

        #endregion
    }
}