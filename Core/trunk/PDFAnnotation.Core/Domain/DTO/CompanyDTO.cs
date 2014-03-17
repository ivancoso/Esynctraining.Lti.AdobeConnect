namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using Esynctraining.Core.Extensions;
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
                this.organizationId = f.OrganizationId;
                this.addressVO = f.Address != null ? new AddressDTO(f.Address) : null;
                this.companyName = f.CompanyName;
                this.dateCreated = f.DateCreated;
                this.dateModified = f.DateModified;
                this.phone = f.Phone;
                this.logoId = f.Logo.Return(x => x.Id, (Guid?)null);
                this.colorPrimary = f.ColorPrimary;
                this.colorSecondary = f.ColorSecondary;
                this.colorText = f.ColorText;
                this.numberOfLicenses = f.NumberOfLicenses;
                this.orderDate = f.OrderDate;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the number of licenses.
        /// </summary>
        [DataMember]
        public int? numberOfLicenses { get; set; }

        /// <summary>
        ///     Gets or sets the order date.
        /// </summary>
        [DataMember]
        public DateTime? orderDate { get; set; }

        /// <summary>
        /// Gets or sets the organization Id.
        /// </summary>
        [DataMember]
        public Guid organizationId { get; set; }

        /// <summary>
        /// Gets or sets the color primary.
        /// </summary>
        [DataMember]
        public string colorPrimary { get; set; }

        /// <summary>
        /// Gets or sets the color secondary.
        /// </summary>
        [DataMember]
        public string colorSecondary { get; set; }

        /// <summary>
        /// Gets or sets the color text.
        /// </summary>
        [DataMember]
        public string colorText { get; set; }

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
        ///     Gets or sets the logo.
        /// </summary>
        [DataMember]
        public Guid? logoId { get; set; }

        /// <summary>
        ///     Gets or sets the company name.
        /// </summary>
        [DataMember]
        public string companyName { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        [DataMember]
        public string phone { get; set; }

        #endregion
    }
}