namespace Esynctraining.Lti.Zoom.Domain.Entities
{
    public class LmsUser
    {
        public virtual LmsCompany LmsCompany { get; set; }

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
    }
}
