namespace EdugameCloud.Lti.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    /// The LMS user.
    /// </summary>
    public class LmsUser : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the company LMS;
        /// </summary>
        public virtual LmsCompany LmsCompany { get; set; }

        /// <summary>
        /// Gets or sets the AC connection mode.
        /// </summary>
        public virtual AcConnectionMode AcConnectionMode { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual string PrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        public virtual string PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters.
        /// </summary>
        public virtual IList<LmsUserParameters> LmsUserParameters { get; protected set; }

        #endregion
    }
}