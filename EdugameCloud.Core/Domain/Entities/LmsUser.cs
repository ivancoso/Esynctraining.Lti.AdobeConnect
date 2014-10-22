namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    /// The LMS user.
    /// </summary>
    public class LmsUser : Entity
    {
        /// <summary>
        /// The LMS user parameters.
        /// </summary>
        private ISet<LmsUserParameters> lmsUserParameters = new HashedSet<LmsUserParameters>();

        #region Public Properties

        /// <summary>
        /// Gets or sets the company LMS;
        /// </summary>
        public virtual CompanyLms CompanyLms { get; set; }

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
        public virtual int UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters.
        /// </summary>
        public virtual ISet<LmsUserParameters> LmsUserParameters
        {
            get
            {
                return this.lmsUserParameters;
            }

            set
            {
                this.lmsUserParameters = value;
            }
        }

        #endregion
    }
}