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
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        public virtual int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the moodle user id.
        /// </summary>
        public virtual int MoodleUserId { get; set; }

        #endregion
    }
}
