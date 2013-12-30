namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The user map.
    /// </summary>
    public class UserActivationMap : BaseClassMap<UserActivation>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserActivationMap"/> class.
        /// </summary>
        public UserActivationMap()
        {
            this.Map(x => x.ActivationCode).Length(50).Not.Nullable();
            this.Map(x => x.DateExpires).Not.Nullable();

            this.References(x => x.User).Not.Nullable();
        }


        #endregion
    }
}