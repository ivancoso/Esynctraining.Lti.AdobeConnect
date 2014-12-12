namespace EdugameCloud.Lti.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The moodle user
    /// </summary>
    public class MoodleUser : Entity
    {
        #region Fields

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public virtual string Domain { get; set; }

        /// <summary>
        /// Gets or sets the moodle user id.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the moodle user id.
        /// </summary>
        public virtual int UserId { get; set; }

        #endregion
    }
}
