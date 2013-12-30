namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The company.
    /// </summary>
    public class Company : Entity
    {
        #region Fields

        /// <summary>
        /// The users.
        /// </summary>
        private ISet<User> users = new HashedSet<User>();

        /// <summary>
        /// The licenses.
        /// </summary>
        private ISet<CompanyLicense> licenses = new HashedSet<CompanyLicense>(); 

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the primary contact.
        /// </summary>
        public virtual User PrimaryContact { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public virtual ISet<User> Users
        {
            get
            {
                return this.users;
            }

            set
            {
                this.users = value;
            }
        }

        /// <summary>
        /// Gets or sets the licenses.
        /// </summary>
        public virtual ISet<CompanyLicense> Licenses
        {
            get
            {
                return this.licenses;
            }

            set
            {
                this.licenses = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual CompanyStatus Status { get; set; }

        #endregion
    }
}