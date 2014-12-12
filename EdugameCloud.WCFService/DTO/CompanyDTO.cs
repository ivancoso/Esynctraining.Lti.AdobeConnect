namespace EdugameCloud.WCFService.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The company DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(UserDTO))]
    [KnownType(typeof(AddressDTO))]
    [KnownType(typeof(CompanyLicenseDTO))]
    [KnownType(typeof(CompanyLmsDTO))]
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
        /// <param name="c">
        /// The c.
        /// </param>
        public CompanyDTO(Company c)
        {
            this.companyId = c.Id;
            this.addressVO = c.Address.Return(x => new AddressDTO(x), null);
            
            this.primaryContactVO = c.PrimaryContact.Return(x => new UserDTO(x), null);
            this.companyName = c.CompanyName;
            this.isActive = c.Status == CompanyStatus.Active;
            this.dateCreated = c.DateCreated;
            this.dateModified = c.DateModified;
            var license = c.CurrentLicense ?? c.FutureActiveLicense;
            this.themeVO = license.Return(x => x.LicenseStatus == CompanyLicenseStatus.Enterprise, false) ? c.Theme.Return(x => new CompanyThemeDTO(c.Id, x), null) : null;
            this.licenseVO = license.Return(x => new CompanyLicenseDTO(x), null);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        [DataMember]
        public AddressDTO addressVO { get; set; }

        /// <summary>
        /// Gets or sets the theme vo.
        /// </summary>
        [DataMember]
        public CompanyThemeDTO themeVO { get; set; }

        /// <summary>
        /// Gets or sets the license vo.
        /// </summary>
        [DataMember]
        public CompanyLicenseDTO licenseVO { get; set; }

        /// <summary>
        /// Gets or sets the contact id.
        /// </summary>
        [DataMember]
        public UserDTO primaryContactVO { get; set; }

        /// <summary>
        /// Gets or sets the primary contact id.
        /// </summary>
        [DataMember]
        public int? primaryContactId { get; set; }

        /// <summary>
        ///     Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [DataMember]
        public string companyName { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the LMS DTO.
        /// </summary>
        [DataMember]
        public CompanyLmsDTO lmsVO { get; set; }

        #endregion
    }
}