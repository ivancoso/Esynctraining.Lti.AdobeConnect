namespace EdugameCloud.WCFService.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.DTO;

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
        public SessionDTO session { get; set; }

        /// <summary>
        /// Gets or sets the splash screen.
        /// </summary>
        [DataMember]
        public SplashScreenDTO splashScreen { get; set; }

        /// <summary>
        /// Gets or sets the company LMS.
        /// </summary>
        [DataMember]
        public CompanyLmsDTO[] companyLms { get; set; }

        [DataMember]
        public bool companyUseEventMapping { get; set; }

        #endregion
    }
}