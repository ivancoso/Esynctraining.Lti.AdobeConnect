namespace Esynctraining.Lti.Lms.Moodle
{
    internal sealed class MoodleSession
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use SSL.
        /// </summary>
        public bool UseSSL { get; set; }

    }
}
