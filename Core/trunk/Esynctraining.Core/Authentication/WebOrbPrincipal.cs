namespace Esynctraining.Core.Authentication
{
    using System.Security.Principal;

    /// <summary>
    ///     The WebOrb principal.
    /// </summary>
    public class WebOrbPrincipal : IPrincipal
    {
        #region Fields

        /// <summary>
        /// The identity.
        /// </summary>
        private readonly IWebOrbIdentity identity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOrbPrincipal"/> class.
        /// </summary>
        /// <param name="identity">
        /// The identity.
        /// </param>
        public WebOrbPrincipal(IWebOrbIdentity identity)
        {
            this.identity = identity;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the identity.
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                return this.identity;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The is in role.
        /// </summary>
        /// <param name="role">
        /// The role.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsInRole(string role)
        {
            return this.identity.Roles.Contains(role.ToLower());
        }

        #endregion
    }
}