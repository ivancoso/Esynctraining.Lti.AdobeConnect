namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The contact type DTO.
    /// </summary>
    [DataContract]
    public class ContactTypeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactTypeDTO" /> class.
        /// </summary>
        public ContactTypeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactTypeDTO"/> class.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public ContactTypeDTO(ContactType c)
        {
            this.contactTypeId = c.Id;
            this.contactType = c.ContactTypeName;
            this.acType = c.ACMappedType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the contact type id.
        /// </summary>
        [DataMember]
        public int contactTypeId { get; set; }

        /// <summary>
        /// Gets or sets the contact type.
        /// </summary>
        [DataMember]
        public string contactType { get; set; }

        /// <summary>
        /// Gets or sets the ac type.
        /// </summary>
        [DataMember]
        public string acType { get; set; }

        #endregion
    }
}