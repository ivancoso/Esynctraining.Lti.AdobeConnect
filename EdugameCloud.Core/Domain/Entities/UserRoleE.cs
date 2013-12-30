namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The user role.
    /// </summary>
    public class UserRole : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the user role name.
        /// </summary>
        public virtual string UserRoleName { get; set; }

        #endregion
    }
}