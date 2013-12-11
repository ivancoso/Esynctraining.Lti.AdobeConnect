namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The company DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(AddressDTO))]
    public class CompanyDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyDTO" /> class.
        /// </summary>
        public CompanyDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyDTO"/> class.
        /// </summary>
        /// <param name="f">
        /// The c.
        /// </param>
        public CompanyDTO(Company f)
        {
            if (f != null)
            {
                this.companyId = f.Id;
                this.companyTypeId = f.CompanyType.Id;
                this.addressVO = f.Address != null ? new AddressDTO(f.Address) : null;
                this.companyName = f.CompanyName;
                this.dateCreated = f.DateCreated;
                this.dateModified = f.DateModified;
                this.phone = f.Phone;
            }
        }

        #endregion

        #region Public Properties

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