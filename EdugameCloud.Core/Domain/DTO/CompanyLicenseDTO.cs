﻿namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using NHibernate.Proxy;

    /// <summary>
    ///     The company license DTO.
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
            this.dateCreated = l.DateCreated;
            this.dateModified = l.DateModified;
            this.expiryDate = l.ExpiryDate;
            this.totalLicensesCount = l.TotalLicensesCount;
            this.isTrial = l.LicenseStatus == CompanyLicenseStatus.Trial;
            this.isPro = l.LicenseStatus == CompanyLicenseStatus.Pro;
            this.isEnterprise = l.LicenseStatus == CompanyLicenseStatus.Enterprise;
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
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [DataMember]
        public string domain { get; set; }

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        [DataMember]
        public DateTime expiryDate { get; set; }

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

        #endregion
    }
}