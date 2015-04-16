namespace EdugameCloud.Lti.BrainHoney
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using EdugameCloud.Lti.API.BrainHoney;

    /// <summary>
    /// The session.
    /// </summary>
    internal class Session //: IBrainHoneySession
    {
        #region Fields

        /// <summary>
        /// The cookies.
        /// </summary>
        private CookieContainer cookies;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class. 
        /// Create a new session object
        /// </summary>
        /// <param name="agent">
        /// User agent to identify this session
        /// </param>
        /// <param name="server">
        /// URL to DLAP server
        /// </param>
        /// <param name="timeout">
        /// Request timeout in milliseconds
        /// </param>
        /// <param name="verbose">
        /// The verbose.
        /// </param>
        public Session(string agent, string server, int timeout = 30000, bool verbose = false)
        {
            this.Agent = agent;
            this.Server = server;
            this.Timeout = timeout;
            this.Verbose = verbose;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the user agent for this application
        ///     You should set this to identify the calling application for logging
        /// </summary>
        public string Agent { get; private set; }

        /// <summary>
        ///     Gets or sets the domain id.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        ///     Gets the URL to the DLAP server
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        ///     Gets or sets request timeout in milliseconds
        ///     Defaults to 30000 (30 seconds)
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether session is verbose or not
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
        public XElement Get(string cmd, params string[] parameters)
        {
            string query = "?cmd=" + cmd;
            for (int index = 0; index + 1 < parameters.Length; index += 2)
            {
                query += "&" + parameters[index] + "=" + parameters[index + 1];
            }

            this.TraceRequest(query, null);
            return this.ReadResponse(this.Request(query, null, null, this.Timeout));
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
            this.cookies = new CookieContainer();
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
            this.TraceRequest(query, xml);
            using (var data = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(data, Encoding.UTF8))
                {
                    xml.WriteTo(writer);
                    writer.Flush();
                    data.Flush();
                    data.Position = 0;
                    return this.ReadResponse(this.Request(query, data, "text/xml", this.Timeout));
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
        private XElement ReadResponse(HttpWebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
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
        /// <param name="timeout">
        /// Request timeout in milliseconds
        /// </param>
        /// <returns>
        /// Http Web Response
        /// </returns>
        private HttpWebResponse Request(string query, Stream postData, string contentType, int timeout)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.Server + query);
            request.UserAgent = this.Agent;
            request.AllowAutoRedirect = false;
            request.CookieContainer = this.cookies;
            request.Timeout = timeout;
            if (timeout > request.ReadWriteTimeout)
            {
                request.ReadWriteTimeout = timeout;
            }

            if (postData != null)
            {
                request.Method = "POST";
                if (!string.IsNullOrEmpty(contentType))
                {
                    request.ContentType = contentType;
                }

                request.ContentLength = postData.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    var buffer = new byte[16 * 1024];
                    int bytes;

                    // post the data
                    while ((bytes = postData.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, bytes);
                    }

                    stream.Close();
                }
            }
            else
            {
                request.Method = "GET";
            }

            var response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        private void Log(string line)
        {
            Console.WriteLine(line);
        }

        /// <summary>
        /// The trace request.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="xml">
        /// The xml.
        /// </param>
        private void TraceRequest(string query, XElement xml)
        {
            if (this.Verbose)
            {
                this.Log("Request: " + this.Server + query);
                if (xml != null)
                {
                    this.Log(xml.ToString());
                }
            }
        }

        /// <summary>
        /// The trace response.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        private void TraceResponse(XElement xml)
        {
            if (this.Verbose)
            {
                this.Log(xml.ToString());
            }
        }

        #endregion
    }
}