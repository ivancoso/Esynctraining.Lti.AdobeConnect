namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The user map.
    /// </summary>
    public class UserRoleMap : BaseClassMap<UserRole>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleMap"/> class.
        /// </summary>
        public UserRoleMap()
        {
            this.Map(x => x.UserRoleName).Length(128).Not.Nullable();
        }


        #endregion
    }
}