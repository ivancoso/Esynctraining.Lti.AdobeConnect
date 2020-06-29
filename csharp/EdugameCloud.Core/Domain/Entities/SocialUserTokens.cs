namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The social user tokens
    /// </summary>
    public class SocialUserTokens : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        public virtual string Provider { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        public virtual string Secret { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}