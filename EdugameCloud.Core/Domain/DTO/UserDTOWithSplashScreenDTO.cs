namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The user DTO.
    /// </summary>
    [DataContract]
    public class UserWithSplashScreenDTO : UserDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserWithSplashScreenDTO" /> class.
        /// </summary>
        public UserWithSplashScreenDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWithSplashScreenDTO"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public UserWithSplashScreenDTO(User user)
            : base(user)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the authentication cookie.
        /// </summary>
        [DataMember]
        public AuthCookieDTO authCookie { get; set; }

        /// <summary>
        /// Gets or sets the splash screen.
        /// </summary>
        [DataMember]
        public SplashScreenDTO splashScreen { get; set; }

        #endregion
    }
}