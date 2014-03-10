namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The user DTO.
    /// </summary>
    [DataContract]
    public class UserDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDTO"/> class.
        /// </summary>
        public UserDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDTO"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public UserDTO(User user)
        {
            if (user != null)
            {
                this.userId = user.Id;
                this.companyId = user.Company.Id;
                this.createdBy = user.CreatedBy.Return(x => x.Id, (int?)null);
                this.modifiedBy = user.ModifiedBy.Return(x => x.Id, (int?)null);
                this.dateCreated = user.DateCreated;
                this.dateModified = user.DateModified;
                this.email = user.Email;
                this.logoId = user.Logo.Return(x => x.Id, (Guid?)null);
                this.firstName = user.FirstName;
                this.languageId = user.Language.Id;
                this.lastName = user.LastName;
                this.timeZoneId = user.TimeZone.Id;
                this.userRoleId = user.UserRole.Id;
                this.isActive = user.Status == UserStatus.Active;
                this.userRole = user.UserRole.With(x => x.UserRoleName);
            }
        }

        #endregion

        #region Public Properties

        [DataMember]
        public Guid? logoId { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        // ReSharper disable ValueParameterNotUsed
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1504:AllAccessorsMustBeMultiLineOrSingleLine", Justification = "Reviewed. Suppression is OK here."), DataMember]
        public virtual string name
        {
            get
            {
                return this.firstName + ' ' + this.lastName;
            }
            set { }
        }
        // ReSharper restore ValueParameterNotUsed

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        [DataMember]
        public string userRole { get; set; }

        /// <summary>
        /// Gets or sets id
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        /// <summary>
        ///     Gets or sets the age of a user.
        /// </summary>
        [DataMember]
        public virtual int companyId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public virtual int? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public virtual int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public virtual DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public virtual DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public virtual string email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public virtual string firstName { get; set; }

        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        [DataMember]
        public virtual int languageId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public virtual bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public virtual string lastName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public virtual string password { get; set; }

        /// <summary>
        /// Gets or sets the time zone id.
        /// </summary>
        [DataMember]
        public virtual int timeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the user role id.
        /// </summary>
        [DataMember]
        public virtual int userRoleId { get; set; }

        #endregion
    }
}