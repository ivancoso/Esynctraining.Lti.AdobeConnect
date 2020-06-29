using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Esynctraining.Core.Logging;

namespace Esynctraining.Lti.Lms.AgilixBuzz
{
    public sealed class BuzzApiClient
    {
        private readonly ILogger _logger;
        private readonly System.Net.Http.HttpClient _httpClient;

        public BuzzApiClient(ILogger logger, System.Net.Http.HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string DomainId { get; private set; }

        #region Public Methods and Operators

        /// <summary>
        /// Returns the error message for a failed DLAP call
        /// </summary>
        private static string GetMessage(XElement result)
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
        private static bool IsSuccess(XElement result)
        {
            return result != null && result.Name == "response" && result.Attribute("code") != null
                   && result.Attribute("code").Value == "OK";
        }

        public async Task<XElement> LoginAsync(string prefix, string username, string password)
        {
            var response = await PostAsync(
                null,
                new XElement(
                    "request",
                    new XAttribute("cmd", "login"),
                    new XAttribute("username", string.Concat(prefix, "/", username)),
                    new XAttribute("password", password)));

            if (!IsSuccess(response))
            {
                var error = "[BuzzApi] Unable to login: " + BuzzApiClient.GetMessage(response);

                _logger.Error(error);

                return null;
            }

            DomainId = response.XPathEvaluate("string(user/@domainid)").ToString();
            return response;
        }

        public async Task<XElement> GetAsync(string cmd, Dictionary<string, string> parameters = null)
        {
            var query = new StringBuilder("?cmd=" + cmd);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query.Append('&');
                    query.Append(param.Key);
                    query.Append('=');
                    query.Append(param.Value);
                }
            }

            var result = await ReadResponseAsync(await MakeRequestAsync(query.ToString(), null, null));
            if (!IsSuccess(result))
            {
                var error = "[BuzzApi] Unable to get api response: " + GetMessage(result);
                _logger.Error(error);
                return null;
            }

            return result;
        }

        /// <summary>
        /// Login to DLAP. Call Logout to close session.
        /// </summary>


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
        private async Task<XElement> PostAsync(string cmd, XElement xml)
        {
            string query = string.IsNullOrEmpty(cmd) ? string.Empty : ("?cmd=" + cmd);
            using (var data = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(data, Encoding.UTF8))
                {
                    xml.WriteTo(writer);
                    writer.Flush();
                    data.Flush();
                    data.Position = 0;

                    return await ReadResponseAsync(await MakeRequestAsync(query, data, "text/xml"));
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
        private async Task<XElement> ReadResponseAsync(HttpResponseMessage response)
        {
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                try
                {
                    XElement result = XElement.Load(stream);
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
        private async Task<HttpResponseMessage> MakeRequestAsync(string query, Stream postData, string contentType)
        {
            var uri = new Uri(query, UriKind.Relative);
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = uri
            };

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

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            return response;
        }

        #endregion

    }
}
