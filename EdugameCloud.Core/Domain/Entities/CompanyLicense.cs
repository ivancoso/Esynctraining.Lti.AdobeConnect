namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The company license.
    /// </summary>
    public class CompanyLicense : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public virtual Company Company { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date start.
        /// </summary>
        public virtual DateTime DateStart { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public virtual string Domain { get; set; }

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        public virtual DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the license number.
        /// </summary>
        public virtual string LicenseNumber { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the total licenses count.
        /// </summary>
        public virtual int TotalLicensesCount { get; set; }

        /// <summary>
        /// Gets or sets the total participants count.
        /// </summary>
        public virtual int TotalParticipantsCount { get; set; }

        /// <summary>
        /// Gets or sets the is license status.
        /// </summary>
        public virtual CompanyLicenseStatus LicenseStatus { get; set; }

        #endregion
    }
}