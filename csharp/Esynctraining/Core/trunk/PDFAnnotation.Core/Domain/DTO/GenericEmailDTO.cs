namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The generic email DTO.
    /// </summary>
    [DataContract]
    public class GenericEmailDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string subject { get; set; }

        /// <summary>
        /// Gets or sets the partner portal url.
        /// </summary>
        [DataMember]
        public string message { get; set; }

        #endregion
    }
}