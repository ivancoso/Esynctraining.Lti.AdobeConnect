namespace EdugameCloud.Core.Domain.Entities
{
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
        /// Gets or sets the first name.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public virtual string Domain { get; set; }

        /// <summary>
        /// Gets or sets the moodle user id.
        /// </summary>
        public virtual int UserId { get; set; }

        #endregion
    }
}
