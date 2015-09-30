namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Utils;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The user.
    /// </summary>
    [DataContract]
    public class User : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ac sessions.
        /// </summary>
        public virtual IList<ACSession> ACSessions { get; protected set; }

        /// <summary>
        /// Gets or sets the activations.
        /// </summary>
        public virtual IList<UserActivation> Activations { get; protected set; }

        /// <summary>
        /// Gets or sets the age of a user.
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
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the distractors created.
        /// </summary>
        public virtual IList<Distractor> DistractorsCreated { get; protected set; }

        /// <summary>
        /// Gets or sets the distractors modified.
        /// </summary>
        public virtual IList<Distractor> DistractorsModified { get; protected set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the ac sessions.
        /// </summary>
        public virtual IList<File> Files { get; protected set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public virtual string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Gets or sets the ac sessions.
        /// </summary>
        public virtual IList<UserLoginHistory> LoginHistory { get; protected set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual File Logo { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets the session
        /// </summary>
        public virtual string SessionToken { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime? SessionTokenExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a status.
        /// </summary>
        public virtual UserStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the sub module categories.
        /// </summary>
        public virtual IList<SubModuleCategory> SubModuleCategories { get; protected set; }

        /// <summary>
        /// Gets or sets the sub module categories modified.
        /// </summary>
        public virtual IList<SubModuleCategory> SubModuleCategoriesModified { get; protected set; }

        /// <summary>
        /// Gets or sets the theme attributes created.
        /// </summary>
        public virtual IList<ThemeAttribute> ThemeAttributesCreated { get; protected set; }

        /// <summary>
        /// Gets or sets the theme attributes modified.
        /// </summary>
        public virtual IList<ThemeAttribute> ThemeAttributesModified { get; protected set; }

        /// <summary>
        /// Gets or sets the time zone id.
        /// </summary>
        public virtual TimeZone TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the user role id.
        /// </summary>
        public virtual UserRole UserRole { get; set; }

        /// <summary>
        /// Gets or sets the unsibscribed.
        /// </summary>
        public virtual bool IsUnsubscribed { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The generate cookie value.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string GenerateCookieValue()
        {
            return this.Id + "G" + this.HashEmail() + "G" + Guid.NewGuid().ToString("N").ToUpper();
        }

        /// <summary>
        ///     The hash email.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string HashEmail()
        {
            return BitConverter.ToString(Cryptographer.GenerateHash(this.Email)).Replace("-", string.Empty).ToUpper();
        }

        /// <summary>
        ///     The is administrator.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool IsAdministrator()
        {
            return ((UserRoleEnum)this.UserRole.Id) == UserRoleEnum.Admin;
        }

        public virtual bool IsSuperAdmin()
        {
            return ((UserRoleEnum)this.UserRole.Id) == UserRoleEnum.SuperAdmin;
        }

        /// <summary>
        /// The is in any role.
        /// </summary>
        /// <param name="roleNames">
        /// The role names.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsInAnyRole(string[] roleNames)
        {
            return roleNames.Select(x => x.ToLower()).Contains(this.UserRole.UserRoleName.ToLower());
        }

        /// <summary>
        /// The is in any role.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsInAnyRole(UserRoleEnum[] roles)
        {
            return roles.Contains((UserRoleEnum)this.UserRole.Id);
        }

        /// <summary>
        /// The is in role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsInRole(string roleName)
        {
            return this.UserRole.UserRoleName.ToLower() == roleName.ToLower();
        }

        /// <summary>
        /// The is in role.
        /// </summary>
        /// <param name="role">
        /// The role name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsInRole(UserRoleEnum role)
        {
            return (UserRoleEnum)this.UserRole.Id == role;
        }

        /// <summary>
        ///     The reset password.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string ResetPassword()
        {
            return this.ResetPassword(AuthenticationModel.CreateRandomPassword);
        }

        /// <summary>
        /// The reset password.
        /// </summary>
        /// <param name="passwordGenerator">
        /// The password generator.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string ResetPassword(Func<int, string> passwordGenerator)
        {
            const int PasswordLength = 8;
            string newPassword = passwordGenerator(PasswordLength);
            this.SetPassword(newPassword);
            return newPassword;
        }

        /// <summary>
        /// The set password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        public virtual void SetPassword(string password)
        {
            this.Password =
                BitConverter.ToString(Cryptographer.GenerateHash(password)).Replace("-", string.Empty).ToUpper();
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

        public User()
        { 
            ACSessions = new List<ACSession>();
            Activations = new List<UserActivation>();
            DistractorsCreated = new List<Distractor>();
            DistractorsModified = new List<Distractor>();
            Files = new List<File>();
            LoginHistory = new List<UserLoginHistory>();
            SubModuleCategories = new List<SubModuleCategory>();
            SubModuleCategoriesModified = new List<SubModuleCategory>();
            ThemeAttributesCreated = new List<ThemeAttribute>();
            ThemeAttributesModified = new List<ThemeAttribute>();
        }

    }

}