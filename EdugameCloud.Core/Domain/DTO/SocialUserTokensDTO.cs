namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The social user tokens DTO.
    /// </summary>
    [DataContract]
    public class SocialUserTokensDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialUserTokensDTO"/> class.
        /// </summary>
        public SocialUserTokensDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialUserTokensDTO"/> class.
        /// </summary>
        /// <param name="sut">
        /// The social tokens.
        /// </param>
        public SocialUserTokensDTO(SocialUserTokens sut)
        {
            this.key = sut.Key;
            this.provider = sut.Provider;
            this.secret = sut.Secret;
            this.token = sut.Token;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [DataMember]
        public string key { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [DataMember]
        public string provider { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        [DataMember]
        public string secret { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        [DataMember]
        public string token { get; set; }

        #endregion
    }
}