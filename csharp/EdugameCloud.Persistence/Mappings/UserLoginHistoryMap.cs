namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The contact login history mapping
    /// </summary>
    public class UserLoginHistoryMap : BaseClassMap<UserLoginHistory>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginHistoryMap"/> class.
        /// </summary>
        public UserLoginHistoryMap()
        {
            this.Map(x => x.Application).Length(255).Nullable();
            this.Map(x => x.FromIP).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();

            this.References(x => x.User).Not.Nullable();
        }

        #endregion
    }
}