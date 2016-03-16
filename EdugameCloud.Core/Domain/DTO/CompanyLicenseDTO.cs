namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The company license DTO.
    /// </summary>
    [DataContract]
    public class CompanyLicenseDTO 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseDTO"/> class.
        /// </summary>
        public CompanyLicenseDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseDTO"/> class.
        /// </summary>
        /// <param name="l">
        /// The l.
        /// </param>
        public CompanyLicenseDTO(CompanyLicense l)
        {
            this.companyLicenseId = l.Id;
            this.companyId = l.Company.Id;
            this.domain = l.Domain;
            this.licenseNumber = l.LicenseNumber;
            this.createdBy = l.CreatedBy.Id;
            this.modifiedBy = l.ModifiedBy.Id;
            this.dateCreated = l.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.dateModified = l.DateModified.With(x => x.ConvertToUnixTimestamp());
            this.startDate = l.DateStart.With(x => x.ConvertToUnixTimestamp());
            this.expiryDate = l.ExpiryDate.With(x => x.ConvertToUnixTimestamp());
            this.totalLicensesCount = l.TotalLicensesCount;
            this.totalParticipantsCount = l.TotalParticipantsCount;
            this.isTrial = l.LicenseStatus == CompanyLicenseStatus.Trial;
            this.isPro = l.LicenseStatus == CompanyLicenseStatus.Pro;
            this.isEnterprise = l.LicenseStatus == CompanyLicenseStatus.Enterprise;
            this.hasApi = l.HasApi;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the company license id.
        /// </summary>
        [DataMember]
        public int companyLicenseId { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is trial.
        /// </summary>
        [DataMember]
        public bool isTrial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is pro.
        /// </summary>
        [DataMember]
        public bool isPro { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is enterprise.
        /// </summary>
        [DataMember]
        public bool isEnterprise { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [DataMember]
        public double startDate { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [DataMember]
        public string domain { get; set; }

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        [DataMember]
        public double expiryDate { get; set; }

        /// <summary>
        /// Gets or sets the license number.
        /// </summary>
        [DataMember]
        public string licenseNumber { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the total licenses count.
        /// </summary>
        [DataMember]
        public int totalLicensesCount { get; set; }

        /// <summary>
        /// Gets or sets the total participants count.
        /// </summary>
        [DataMember]
        public int totalParticipantsCount { get; set; }

        [DataMember]
        public bool hasApi { get; set; }

        #endregion

    }

}
