namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The category contact dto.
    /// </summary>
    [DataContract]
    public class CategoryContactDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the category id.
        /// </summary>
        [DataMember]
        public virtual int categoryId { get; set; }

        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [DataMember]
        public virtual int contactId { get; set; }

        #endregion
    }
}