namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The social user tokens
    /// </summary>
    public class SocialUserTokensMap : BaseClassMap<SocialUserTokens>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialUserTokensMap"/> class. 
        /// </summary>
        public SocialUserTokensMap()
        {
            this.Map(x => x.Key).Length(255).Nullable();
            this.Map(x => x.Token).Length(1000).Not.Nullable();
            this.Map(x => x.Provider).Length(500).Not.Nullable();
            this.Map(x => x.Secret).Length(1000).Nullable();

            this.References(x => x.User).Nullable();
        }

        #endregion
    }
}