﻿namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Esynctraining.Core.Domain.Entities;


    /// <summary>
    /// The company.
    /// </summary>
    public class Company : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public virtual Address Address { get; set; }

        public virtual CompanyTheme Theme { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public virtual string CompanyName { get; set; }

        public virtual bool UseEventMapping { get; set; }

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

        public virtual CompanyLicense CurrentLicense
        {
            get
            {
                return this.Licenses.ToList().Where(x => x.DateStart <= DateTime.Now && x.ExpiryDate >= DateTime.Now).OrderByDescending(x => (int)x.LicenseStatus).ThenByDescending(x => x.DateCreated).FirstOrDefault();
            }
            set
            {

            }
        }

        public virtual CompanyLicense FutureActiveLicense
        {
            get
            {
                return this.Licenses.FirstOrDefault(x => x.ExpiryDate >= DateTime.Now);
            }
            set
            {

            }
        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public virtual IList<User> Users { get; protected set; }

        /// <summary>
        /// Gets or sets the licenses.
        /// </summary>
        public virtual IList<CompanyLicense> Licenses { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual CompanyStatus Status { get; set; }
        
        #endregion

        public Company()
        {
            Licenses = new List<CompanyLicense>();
            Users = new List<User>();
        }


        public virtual bool IsActive()
        {
            return Status == CompanyStatus.Active
                && CurrentLicense != null
                && CurrentLicense.ExpiryDate.ToUniversalTime() > DateTime.UtcNow;
        }

    }

}