namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     The config DTO.
    /// </summary>
    [DataContract]
    public class ConfigDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the base url.
        /// </summary>
        [DataMember]
        public string baseUrl { get; set; }

        /// <summary>
        /// Gets or sets the file service.
        /// </summary>
        [DataMember]
        public string fileService { get; set; }

        /// <summary>
        /// Gets or sets the forgot password url.
        /// </summary>
        [DataMember]
        public string forgotPasswordUrl { get; set; }

        /// <summary>
        /// Gets or sets the get image.
        /// </summary>
        [DataMember]
        public string getImage { get; set; }

        /// <summary>
        /// Gets or sets the get PDF.
        /// </summary>
        [DataMember]
        public string getPDF { get; set; }

        /// <summary>
        /// Gets or sets the get PDF archive.
        /// </summary>
        [DataMember]
        public string getPDFArchive { get; set; }

        #endregion
    }
}