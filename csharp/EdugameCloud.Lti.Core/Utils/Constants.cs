namespace EdugameCloud.Lti.Core.Utils
{
    public static class Constants
    {
        /// <summary>
        ///     The return uri extension parameter.
        /// </summary>
        public const string ReturnUriExtensionQueryParameterName = "providerUrl";

        public const int SyncUsersCountLimit = 1000;

        public const int MoodleUsersApiRequestTimeout = 3 * 60 * 1000; //3 minutes, in milliseconds
    }

}
