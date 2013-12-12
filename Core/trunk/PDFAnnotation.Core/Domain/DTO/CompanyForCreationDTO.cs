namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The company DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ContactDTO))]
    [KnownType(typeof(AddressDTO))]
    public class CompanyForCreationDTO
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the contact.
        /// </summary>
        [DataMember]
        public ContactDTO contactVO { get; set; }

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        [DataMember]
        public AddressDTO addressVO { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime? dateModified { get; set; }

        /// <summary>
        ///     Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        ///     Gets or sets the company name.
        /// </summary>
        [DataMember]
        public string companyName { get; set; }

        /// <summary>
        ///     Gets or sets the company type id.
        /// </summary>
        [DataMember]
        public int companyTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        [DataMember]
        public string phone { get; set; }

        #endregion
    }
}