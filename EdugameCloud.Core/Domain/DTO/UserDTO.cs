namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

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
                this.dateCreated = user.DateCreated.ConvertToUnixTimestamp();
                this.dateModified = user.DateModified.ConvertToUnixTimestamp();
                this.email = user.Email;
                this.logoId = user.Logo.Return(x => x.Id, (Guid?)null);
                this.firstName = user.FirstName;
                this.languageId = user.Language.Id;
                this.lastName = user.LastName;
                this.timeZoneId = user.TimeZone.Id;
                this.userRoleId = user.UserRole.Id;
                this.isActive = user.Status == UserStatus.Active;
                this.userRole = user.UserRole.With(x => x.UserRoleName);
                this.isUnsubscribed = user.IsUnsubscribed;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the logo id.
        /// </summary>
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
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the age of a user.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        [DataMember]
        public int languageId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember]
        public string password { get; set; }

        /// <summary>
        /// Gets or sets the time zone id.
        /// </summary>
        [DataMember]
        public int timeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the user role id.
        /// </summary>
        [DataMember]
        public int userRoleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is unsubscribed.
        /// </summary>
        [DataMember]
        public bool isUnsubscribed { get; set; }

        #endregion
    }
}