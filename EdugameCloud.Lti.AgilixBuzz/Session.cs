namespace EdugameCloud.Lti.AgilixBuzz
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using EdugameCloud.HttpClient;
    using Esynctraining.Core.Logging;

    internal sealed class Session
    {
        private readonly ILogger _logger;
        private HttpClientWrapper _httpClientWrapper;

        #region Constructors and Destructors

        public Session(ILogger logger, string agent, string server, int timeout = 30000, bool verbose = false)
        {
            _logger = logger;
            Agent = agent;
            Server = server;
            Timeout = TimeSpan.FromMilliseconds(timeout);
            Verbose = verbose;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the user agent for this application
        ///     You should set this to identify the calling application for logging
        /// </summary>
        public string Agent { get; private set; }

        /// <summary>
        /// Gets or sets the domain id.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Gets the URL to the DLAP server
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        /// Gets or sets request timeout in milliseconds
        ///     Defaults to 30000 (30 seconds)
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether session is verbose or not
        /// </summary>
        public bool Verbose { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the error message for a failed DLAP call
        /// </summary>
        /// <param name="result">
        /// XML results
        /// </param>
        /// <returns>
        /// Error message as given by DLAP
        /// </returns>
        public static string GetMessage(XElement result)
        {
            if (result != null && result.Name == "response" && result.Attribute("message") != null)
            {
                return result.Attribute("message").Value;
            }

            return "Unknown error";
        }

        /// <summary>
        /// Checks if the DLAP call was successful
        /// </summary>
        /// <param name="result">
        /// XML result
        /// </param>
        /// <returns>
        /// TRUE is successful, otherwise false
        /// </returns>
        public static bool IsSuccess(XElement result)
        {
            return result != null && result.Name == "response" && result.Attribute("code") != null
                   && result.Attribute("code").Value == "OK";
        }

        /// <summary>
        /// Makes a GET request to DLAP
        /// </summary>
        /// <param name="cmd">
        /// DLAP command
        /// </param>
        /// <param name="parameters">
        /// pairs of name-value for additional parameters
        /// </param>
        /// <returns>
        /// XML results
        /// </returns>
        public XElement Get(string cmd, Dictionary<string, string> parameters = null)
        {
            StringBuilder query = new StringBuilder("?cmd=" + cmd);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query.Append("&");
                    query.Append(param.Key);
                    query.Append("=");
                    query.Append(param.Value);
                }
            }

            TraceRequest(query.ToString(), null);
            return ReadResponse(this.Request(query.ToString(), null, null));
        }

        /// <summary>
        /// Login to DLAP. Call Logout to close session.
        /// </summary>
        /// <param name="prefix">
        /// Login prefix
        /// </param>
        /// <param name="username">
        /// User name
        /// </param>
        /// <param name="password">
        /// The password
        /// </param>
        /// <returns>
        /// XML results
        /// </returns>
        public XElement Login(string prefix, string username, string password)
        {
            _httpClientWrapper = new HttpClientWrapper(Timeout, new CookieContainer(), false);

            return this.Post(
                null, 
                new XElement(
                    "request", 
                    new XAttribute("cmd", "login"), 
                    new XAttribute("username", string.Concat(prefix, "/", username)), 
                    new XAttribute("password", password)));
        }

        ///// <summary>
        /////     Logout of DLAP
        ///// </summary>
        ///// <returns>XML results</returns>
        //public XElement Logout()
        //{
        //    XElement result = this.Get("logout");
        //    this.cookies = null;
        //    return result;
        //}

        /// <summary>
        /// Makes a POST request to DLAP
        /// </summary>
        /// <param name="cmd">
        /// DLAP command
        /// </param>
        /// <param name="xml">
        /// XML to post to DLAP
        /// </param>
        /// <returns>
        /// XML results
        /// </returns>
        public XElement Post(string cmd, XElement xml)
        {
            string query = string.IsNullOrEmpty(cmd) ? string.Empty : ("?cmd=" + cmd);
            TraceRequest(query, xml);
            using (var data = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(data, Encoding.UTF8))
                {
                    xml.WriteTo(writer);
                    writer.Flush();
                    data.Flush();
                    data.Position = 0;
                    return ReadResponse(Request(query, data, "text/xml"));
                }
            }
        }

        ///// <summary>
        ///// Makes a POST request to DLAP
        ///// </summary>
        ///// <param name="cmd">
        ///// DLAP command
        ///// </param>
        ///// <param name="xml">
        ///// XML to post to DLAP
        ///// </param>
        ///// <returns>
        ///// XML results
        ///// </returns>
        //public XElement Post(string cmd, string xml)
        //{
        //    if (!xml.StartsWith("<request"))
        //    {
        //        xml = "<requests>" + xml + "</requests>";
        //    }

        //    return this.Post(cmd, XElement.Parse(xml));
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Returns the XML data when calling RawRequest
        /// </summary>
        /// <param name="response">
        /// HttpWebResponse returned from RawRequest
        /// </param>
        /// <returns>
        /// XML results
        /// </returns>
        private XElement ReadResponse(HttpResponseMessage response)
        {
            using (Stream stream = response.Content.ReadAsStreamAsync().Result)
            {
                try
                {
                    XElement result = XElement.Load(stream);
                    this.TraceResponse(result);
                    return result;
                }
                catch (Exception e)
                {
                    return new XElement(
                        "response",
                        new XAttribute("code", e.GetType().Name),
                        new XAttribute("message", e.Message));
                }
            }
        }

        /// <summary>
        /// Makes a raw request to DLAP. Use the Request methods when for XML data
        /// </summary>
        /// <param name="query">
        /// Full query string for the request
        /// </param>
        /// <param name="postData">
        /// Optional post data, if present the request is a POST rather than a get.
        /// </param>
        /// <param name="contentType">
        /// Content type of the post data
        /// </param>
        /// <returns>
        /// Http Web Response
        /// </returns>
        private HttpResponseMessage Request(string query, Stream postData, string contentType)
        {
            var requestMessage = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri(Server + query)
            };

            requestMessage.Headers.UserAgent.ParseAdd(Agent);

            if (postData != null)
            {
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StreamContent(postData);
                
                if (!string.IsNullOrEmpty(contentType))
                {
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
            }
            else
            {
                requestMessage.Method = HttpMethod.Get;
            }

            var response = _httpClientWrapper.SendAsync(requestMessage).Result;

            response.EnsureSuccessStatusCode();

            return response;
        }

        private void Log(string line)
        {
            _logger.Debug(line);
        }

        private void TraceRequest(string query, XElement xml)
        {
            if (Verbose)
            {
                Log("Request: " + Server + query);
                if (xml != null)
                {
                    Log(xml.ToString());
                }
            }
        }

        private void TraceResponse(XElement xml)
        {
            if (Verbose)
            {
                Log(xml.ToString());
            }
        }

        #endregion

    }

}