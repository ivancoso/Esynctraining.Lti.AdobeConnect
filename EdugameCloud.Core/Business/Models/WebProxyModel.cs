namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Castle.Core.Logging;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Providers;

    /// <summary>
    /// The web proxy model.
    /// </summary>
    public class WebProxyModel
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebProxyModel"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public WebProxyModel(ApplicationSettingsProvider settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="webParams">
        /// The web parameters.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Get(WebRequestDTO webParams, out bool result)
        {
            string strResponse;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(webParams.url);
                req.Method = "GET";
                if (!string.IsNullOrWhiteSpace(webParams.contentType))
                {
                    req.ContentType = webParams.contentType;
                }

                if (webParams.headers != null && webParams.headers.Any())
                {
                    foreach (var header in webParams.headers)
                    {
                        req.Headers.Add(header.name, header.value);
                    }
                }

                var res = req.GetResponse();

                var inputStream = res.GetResponseStream();
                if (inputStream != null)
                {
                    using (var inputStreamReader = new StreamReader(inputStream))
                    {
                        strResponse = inputStreamReader.ReadToEnd();
                        inputStreamReader.Close();
                    }
                    
                }
                else
                {
                    strResponse = string.Empty;
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                strResponse = ex.ToString();
                this.logger.Error("Post error: ", ex);
            }

            return strResponse;
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="webParams">
        /// The web parameters.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Post(WebRequestDTO webParams, out bool result)
        {
            string strResponse;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(webParams.url);
                req.KeepAlive = false;
                req.Method = "POST";
                req.ContentType = string.IsNullOrWhiteSpace(webParams.contentType)
                                      ? "application/json"
                                      : webParams.contentType;
                req.ContentLength = webParams.data.Length;
                if (webParams.headers != null && webParams.headers.Any())
                {
                    foreach (var header in webParams.headers)
                    {
                        req.Headers.Add(header.name, header.value);
                    }
                }

                using (var outStream = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                {
                    outStream.Write(webParams.data);
                    outStream.Close();

                    var res = req.GetResponse();
                    var inputStream = res.GetResponseStream();
                    if (inputStream != null)
                    {
                        using (var inputStreamReader = new StreamReader(inputStream))
                        {
                            strResponse = inputStreamReader.ReadToEnd();
                            inputStreamReader.Close();
                        }
                    }
                    else
                    {
                        strResponse = string.Empty;
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                strResponse = ex.ToString();
                this.logger.Error("Post error: ", ex);
            }

            return strResponse;
        }

        #endregion
    }
}
