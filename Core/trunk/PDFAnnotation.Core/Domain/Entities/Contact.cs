namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using System.Linq;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.FullText;
    using Esynctraining.Core.Utils;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The contact.
    /// </summary>
    [FullTextEnabled]
    public class Contact : Entity
    {
        #region Fields

        /// <summary>
        /// The email.
        /// </summary>
        private string email;

        /// <summary>
        ///     The passwordActivations.
        /// </summary>
        private ISet<PasswordActivation> passwordActivations = new HashedSet<PasswordActivation>();

        /// <summary>
        ///     The categories.
        /// </summary>
        private ISet<Category> categories = new HashedSet<Category>();

        /// <summary>
        ///     The companyContacts.
        /// </summary>
        private ISet<CompanyContact> companyContacts = new HashedSet<CompanyContact>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the principalId.
        /// </summary>
        public virtual string ACPrincipalId { get; set; }

        /// <summary>
        ///     Gets or sets is super admin.
        /// </summary>
        public virtual bool IsSuperAdmin { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime? DateModified { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        [FullTextIndexed(2)]
        public virtual string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (!string.IsNullOrEmpty(this.email))
                {
                    this.PreviousEmail = this.email;
                }

                this.email = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        ///     Gets or sets the passwordActivations.
        /// </summary>
        public virtual ISet<PasswordActivation> PasswordActivation
        {
            get
            {
                return this.passwordActivations;
            }

            set
            {
                this.passwordActivations = value;
            }
        }

        /// <summary>
        ///     Gets or sets the categories.
        /// </summary>
        public virtual ISet<Category> Categories
        {
            get
            {
                return this.categories;
            }

            set
            {
                this.categories = value;
            }
        }

        /// <summary>
        ///     Gets or sets the categories.
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

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        [FullTextIndexed(0)]
        public virtual string FirstName { get; set; }

        /// <summary>
        ///     Gets the full name.
        /// </summary>
        public virtual string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        [FullTextIndexed(1)]
        public virtual string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the mobile phone.
        /// </summary>
        public virtual string MobilePhone { get; set; }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        public virtual string OfficePhone { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        ///     Gets or sets the plain password.
        /// </summary>
        public virtual string PlainPassword { get; set; }

        /// <summary>
        ///     Gets or sets property that would be visible only during change of email
        /// </summary>
        public virtual string PreviousEmail { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        public virtual string PreviousPassword { get; set; }

        /// <summary>
        ///     Gets or sets the plain password.
        /// </summary>
        public virtual int? RBContactId { get; set; }

        /// <summary>
        ///     Gets or sets a value for status.
        /// </summary>
        public virtual ContactStatusEnum Status { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether update ac.
        /// </summary>
        public virtual bool UpdateAC { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The set password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="previousPassword">
        /// The previous Password.
        /// </param>
        public virtual void SetPassword(string password, string previousPassword)
        {
            this.PreviousPassword = string.IsNullOrWhiteSpace(previousPassword)
                                        ? "01temp23password45"
                                        : previousPassword;
            this.Password =
                BitConverter.ToString(Cryptographer.GenerateHash(password)).Replace("-", string.Empty).ToUpper();
            this.PlainPassword = password;
            this.UpdateAC = true;
        }

        /// <summary>
        /// The validate password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return this.Password.ToLower()
                   == BitConverter.ToString(Cryptographer.GenerateHash(password)).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// The validate password.
        /// </summary>
        /// <param name="passwordHash">
        /// The password Hash.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool ValidatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            return this.Password.ToLower() == passwordHash.ToLower();
        }

        #endregion
    }
}