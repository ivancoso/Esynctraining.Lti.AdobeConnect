namespace Esynctraining.Lti.Lms.Common.Constants
{
    /// <summary>
    /// The http scheme.
    /// </summary>
    public static class HttpScheme
    {
        /// <summary>
        /// The http.
        /// </summary>
        public const string Http = "http://";

        /// <summary>
        /// The https.
        /// </summary>
        public const string Https = "https://";
    }

    public static class Http
    {
        public const string MoodleApiClientName = "MoodleApiClient";
        public const double MoodleApiClientTimeout = 3 * 60 * 1000;
        public const double BuzzApiClientTimeout = 30 * 1000;
    }
}
