// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessor.cs" company="eSyncTraining">
//   eSyncTraining
// </copyright>
// <summary>
//   Defines the RequestProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Esynctraining.AC.Provider.Utils
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The request processor.
    /// </summary>
    internal class RequestProcessor
    {
        #region Private Fields

        /// <summary>
        /// The _connection details.
        /// </summary>
        private readonly ConnectionDetails connectionDetails;

        /// <summary>
        /// The session cookie.
        /// </summary>
        private Cookie sessionCookie;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessor" /> class.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="sessionId">The session id.</param>
        public RequestProcessor(ConnectionDetails details, string sessionId)
        {
            if (details == null || string.IsNullOrWhiteSpace(details.ServiceUrl))
            {
                throw new ArgumentException("ConnectionDetails was not properly initialized", "details");
            }

            this.connectionDetails = details;
            this.SetSessionId(sessionId);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the service url.
        /// </summary>
        public string ServiceUrl
        {
            get
            {
                return this.connectionDetails != null ? this.connectionDetails.ServiceUrl : null;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets a value indicating whether is session cookie valid.
        /// </summary>
        protected bool IsSessionCookieValid
        {
            get { return this.sessionCookie != null && !string.IsNullOrWhiteSpace(this.sessionCookie.Value) && !string.IsNullOrWhiteSpace(this.sessionCookie.Domain); }
        }

        /// <summary>
        /// Gets a value indicating whether is logged in.
        /// </summary>
        protected bool IsLoggedIn
        {
            // TODO: check if session is not expired!
            get { return this.IsSessionCookieValid; }
        }

        #endregion
        
        /// <summary>
        /// The set session cookie.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        public void SetSessionId(string sessionId)
        {
            this.sessionCookie = new Cookie(AdobeConnectProviderConstants.SessionCookieName, sessionId, "/", new Uri(this.connectionDetails.ServiceUrl).Host);
        }

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        public XmlDocument Process(string action, string parameters, out StatusInfo status)
        {
            status = new StatusInfo { Code = StatusCodes.not_set };

            if (parameters == null)
            {
                parameters = string.Empty;
            }

            var webRequest = this.CreateWebRequest(action, parameters);

            if (webRequest == null)
            {
                return null;
            }

            HttpWebResponse webResponse = null;

            try
            {
                // FIX: invalid SSL passing behavior
                // (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                webResponse = webRequest.GetResponse() as HttpWebResponse;

                if (webResponse == null)
                {
                    return null;
                }

                if (!this.IsLoggedIn)
                {
                    if (webResponse.Cookies[AdobeConnectProviderConstants.SessionCookieName] != null)
                    {
                        this.sessionCookie = webResponse.Cookies[AdobeConnectProviderConstants.SessionCookieName];

                        status.SessionInfo = this.sessionCookie.Value;
                    }
                }

                var receiveStream = webResponse.GetResponseStream();

                if (receiveStream == null)
                {
                    return null;
                }

                XmlDocument doc = null;

                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    var buf = readStream.ReadToEnd();

                    if (!string.IsNullOrEmpty(buf))
                    {
                        doc = new XmlDocument();
                        doc.Load(new StringReader(buf));

                        status.InnerXml = doc.InnerXml;

                        status.Code = EnumReflector.ReflectEnum(doc.SelectSingleNodeValue("//status/@code"), StatusCodes.not_set);

                        switch (status.Code)
                        {
                            case StatusCodes.invalid:
                                // there is not always an invalid child element
                                var node = doc.SelectSingleNode("//invalid");

                                if (node != null)
                                {
                                    status.SubCode = EnumReflector.ReflectEnum(node.SelectAttributeValue("subcode"), StatusSubCodes.not_set);
                                    status.InvalidField = node.SelectAttributeValue("field");
                                }

                                break;

                            case StatusCodes.no_access:
                                status.SubCode = EnumReflector.ReflectEnum(doc.SelectSingleNodeValue("//status/@subcode"), StatusSubCodes.not_set);
                                break;
                        }
                    }
                }

                return doc;
            }
            catch (Exception ex)
            {
                webRequest.Abort();
                TraceTool.TraceException(ex);
                status.UnderlyingExceptionInfo = ex;
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// The create web request.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="HttpWebRequest"/>.
        /// </returns>
        private HttpWebRequest CreateWebRequest(string action, string parameters)
        {
            var request = WebRequest.Create(this.connectionDetails.ServiceUrl + string.Format(@"?action={0}&{1}", action, parameters)) as HttpWebRequest;

            if (request == null)
            {
                return null;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Url))
                {
                    if (!string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Login) && !string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Password))
                    {
                        request.Proxy = new WebProxy(this.connectionDetails.Proxy.Url, true)
                        {
                            Credentials =
                                new NetworkCredential(
                                this.connectionDetails.Proxy.Login,
                                this.connectionDetails.Proxy.Password,
                                this.connectionDetails.Proxy.Domain)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            // 20 sec. timeout: A Domain Name System (DNS) query may take up to 15 seconds to return or time out.
            request.Timeout = 20000 * 60;
            request.Accept = "*/*";
            request.KeepAlive = false;
            request.CookieContainer = new CookieContainer();

            if (this.IsLoggedIn)
            {
                request.CookieContainer.Add(this.sessionCookie);
            }

            return request;
        }
    }
}
