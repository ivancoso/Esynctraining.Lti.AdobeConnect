namespace EdugameCloud.Lti.API.Moodle
{
    /// <summary>
    /// The Moodle session.
    /// </summary>
    public sealed class MoodleSession
    {
        #region Public Properties

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

        #endregion
    }
}