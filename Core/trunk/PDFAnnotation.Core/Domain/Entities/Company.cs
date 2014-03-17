namespace PDFAnnotation.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The company.
    /// </summary>
    public class Company : Entity
    {

        /// <summary>
        ///     The companyContacts.
        /// </summary>
        private ISet<CompanyContact> companyContacts = new HashedSet<CompanyContact>();

        #region Public Properties

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Gets or sets the color primary.
        /// </summary>
        public virtual string ColorPrimary { get; set; }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        public virtual DateTime? OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the number of licenses.
        /// </summary>
        public virtual int? NumberOfLicenses { get; set; }

        /// <summary>
        /// Gets or sets the organization Id.
        /// </summary>
        public virtual Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the color secondary.
        /// </summary>
        public virtual string ColorSecondary { get; set; }

        /// <summary>
        /// Gets or sets the color text.
        /// </summary>
        public virtual string ColorText { get; set; }

        /// <summary>
        ///     Gets or sets the company name.
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime? DateModified { get; set; }

        /// <summary>
        ///     Gets or sets the logo.
        /// </summary>
        public virtual File Logo { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        ///     Gets or sets the rb company id.
        /// </summary>
        public virtual int? RBFirmId { get; set; }

        /// <summary>
        ///     Gets or sets the companyContacts.
        /// </summary>
        public virtual ISet<CompanyContact> CompanyContacts
        {
            get
            {
                return this.companyContacts;
            }

            set
            {
                this.companyContacts = value;
            }
        }

        #endregion
    }
}