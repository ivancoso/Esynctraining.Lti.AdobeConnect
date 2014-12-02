namespace EdugameCloud.Lti.API.BrainHoney
{
    /// <summary>
    ///     The brain honey enrollment.
    /// </summary>
    public class Enrollment
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        #endregion
    }
}