namespace EdugameCloud.Lti.WebRequester
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using log4net;
    using log4net.Config;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Public Methods and Operators

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            string url = args[0];
            if (!string.IsNullOrEmpty(url))
            {
                WebRequest(url);
            }

        }

        /// <summary>
        /// The web request.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        public static void WebRequest(string url)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // Create a request for the URL.
                WebRequest request = System.Net.WebRequest.Create(url);

                // Get the response.
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Close open connections
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetLogger(typeof(Program));
                logger.Error("Error during requesting url: " + url, ex);

            }
        }

        #endregion
    }
}