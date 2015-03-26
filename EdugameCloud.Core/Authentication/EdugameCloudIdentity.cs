namespace EdugameCloud.Core.Authentication
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Authentication;

    /// <summary>
    ///     The Edugame cloud identity.
    /// </summary>
    public sealed class EdugameCloudIdentity : IWebOrbIdentity
    {
        #region Fields

        /// <summary>
        ///     The roles.
        /// </summary>
        private readonly List<string> roles = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameCloudIdentity"/> class.
        /// </summary>
        /// <param name="userEntity">
        /// The user entity.
        /// </param>
        public EdugameCloudIdentity(User userEntity)
        {
            this.Name = userEntity.Email;
            this.InternalId = userEntity.Id;
            this.InternalEntity = userEntity;
            this.roles.Add(userEntity.UserRole.UserRoleName.ToLower());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdugameCloudIdentity"/> class.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        public EdugameCloudIdentity(string email)
        {
            this.Name = email;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EdugameCloudIdentity" /> class.
        /// </summary>
        protected EdugameCloudIdentity()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the authentication type.
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "UserName";
            }
        }

        /// <summary>
        ///     Gets or sets the internal id.
        /// </summary>
        public int? InternalId { get; set; }

        /// <summary>
        ///     Gets or sets the internal id.
        /// </summary>
        public User InternalEntity { get; set; }

        /// <summary>
        ///     Gets a value indicating whether is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        public List<string> Roles
        {
            get
            {
                return this.roles;
            }
        }

        #endregion

    }

}