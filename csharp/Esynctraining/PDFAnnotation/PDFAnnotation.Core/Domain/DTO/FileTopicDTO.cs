namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The file topic DTO.
    /// </summary>
    [DataContract]
    public class FileTopicDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        [DataMember]
        public string fileId { get; set; }

        /// <summary>
        /// Gets or sets the topic id.
        /// </summary>
        [DataMember]
        public int topicId { get; set; }

        #endregion
    }
}