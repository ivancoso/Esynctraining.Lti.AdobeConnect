namespace Esynctraining.AC.Provider.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Xml;

    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    ///     The request processor.
    /// </summary>
    internal class RequestProcessor
    {
        #region Fields

        /// <summary>
        ///     The _connection details.
        /// </summary>
        private readonly ConnectionDetails connectionDetails;

        /// <summary>
        ///     The session cookie.
        /// </summary>
        private Cookie sessionCookie;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessor"/> class.
        /// </summary>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
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
        ///     Gets the service url.
        /// </summary>
        public string ServiceUrl
        {
            get
            {
                return this.connectionDetails != null ? this.connectionDetails.ServiceUrl : null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether is logged in.
        /// </summary>
        protected bool IsLoggedIn
        {
            // TODO: check if session is not expired!
            get
            {
                return this.IsSessionCookieValid;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether is session cookie valid.
        /// </summary>
        protected bool IsSessionCookieValid
        {
            get
            {
                return this.sessionCookie != null && !string.IsNullOrWhiteSpace(this.sessionCookie.Value)
                       && !string.IsNullOrWhiteSpace(this.sessionCookie.Domain);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get download pattern.
        /// </summary>
        /// <param name="downloadName">
        /// The download Name.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public byte[] DownloadData(string downloadName, string format, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/output/{1}.{2}?download={2}", 
                this.connectionDetails.ServiceUrl.Replace(@"api/xml", string.Empty).Trim('/'), 
                downloadName, 
                format);
           
              return DownloadData(url, out error);
        }

        public byte[] DownloadData2(string urlPath, string fileName, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/output/{2}?download={2}",
                this.connectionDetails.ServiceUrl.Replace(@"api/xml", string.Empty).Trim('/'),
                urlPath,
                fileName);

            return DownloadData(url, out error);
        }

        public byte[] DownloadSourceData(string urlPath, string fileName, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/source/{2}?download={2}",
                this.connectionDetails.ServiceUrl.Replace(@"api/xml", string.Empty).Trim('/'),
                urlPath,
                fileName);

            return DownloadData(url, out error);
        }

        private byte[] DownloadData(string url, out string error)
        {
            error = null;            
            var request = WebRequest.Create(url) as HttpWebRequest;
            request = this.ProcessRequest(request);
            if (request != null)
            {
                HttpWebResponse webResponse = null;

                try
                {
                    // FIX: invalid SSL passing behavior
                    // (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    webResponse = request.GetResponse() as HttpWebResponse;

                    if (webResponse == null)
                    {
                        return null;
                    }

                    Stream receiveStream = webResponse.GetResponseStream();

                    if (receiveStream == null)
                    {
                        return null;
                    }

                    using (var ms = new MemoryStream())
                    {
                        receiveStream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    request.Abort();
                    TraceTool.TraceException(ex);
                    error = ex.ToString();
                }
                finally
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                }
            }

            return null;
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

            HttpWebRequest webRequest = this.CreateWebRequest(action, parameters);

            if (webRequest == null)
            {
                return null;
            }

            return this.FinalizeProcessingResponse(status, webRequest);
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
        /// <param name="uploadData">
        /// The upload Data.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        public XmlDocument ProcessUpload(
            string action,
            string parameters,
            UploadScoInfo uploadData,
            out StatusInfo status)
        {
            status = new StatusInfo { Code = StatusCodes.not_set };

            if (parameters == null)
            {
                parameters = string.Empty;
            }

            try
            {
                HttpWebRequest webRequest = this.CreateWebRequest(action, parameters);
                var httpClient = new HttpClient();
                // ReSharper disable once UseObjectOrCollectionInitializer
                var form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(uploadData.fileBytes, 0, uploadData.fileBytes.Length), "file", uploadData.fileName);
                var response = httpClient.PostAsync(webRequest.RequestUri + "&session=" + this.sessionCookie.Value, form).Result;
                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                var result = response.Content.ReadAsStringAsync().Result;
                return this.ProcessXmlResult(status, result);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
                return null;
            }
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
        /// <param name="uploadData">
        /// The upload Data.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        public XmlDocument ProcessUploadMultipart(
            string action, 
            string parameters, 
            UploadScoInfo uploadData, 
            out StatusInfo status)
        {
            status = new StatusInfo { Code = StatusCodes.not_set };

            if (parameters == null)
            {
                parameters = string.Empty;
            }

            HttpWebRequest webRequest = this.CreateWebRequest(action, parameters);

            if (webRequest == null || uploadData == null)
            {
                return null;
            }

            try
            {
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webRequest.Method = "POST";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    this.WriteMultipartForm(
                        requestStream, 
                        boundary, 
                        new Dictionary<string, string>(), 
                        uploadData.fileName, 
                        uploadData.fileContentType, 
                        uploadData.fileBytes);
                }

                return this.FinalizeProcessingResponse(status, webRequest);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
                return null;
            }
        }

        /// <summary>
        /// The set session cookie.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void SetSessionId(string sessionId)
        {
            this.sessionCookie = new Cookie(
                AdobeConnectProviderConstants.SessionCookieName, 
                sessionId, 
                "/", 
                new Uri(this.connectionDetails.ServiceUrl).Host);
        }

        #endregion

        #region Methods

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
            var request =
                WebRequest.Create(
                    this.connectionDetails.ServiceUrl + string.Format(@"?action={0}&{1}", action, parameters)) as
                HttpWebRequest;

            return this.ProcessRequest(request);
        }

        /// <summary>
        /// The finalize processing response.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="webRequest">
        /// The web request.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        private XmlDocument FinalizeProcessingResponse(StatusInfo status, HttpWebRequest webRequest)
        {
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
                
                Stream receiveStream = webResponse.GetResponseStream();

                if (receiveStream == null)
                {
                    return null;
                }

                XmlDocument doc = null;

                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    string buf = readStream.ReadToEnd();

                    if (!string.IsNullOrEmpty(buf))
                    {
                        doc = this.ProcessXmlResult(status, buf);
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
        /// The process xml result.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        private XmlDocument ProcessXmlResult(StatusInfo status, string buffer)
        {
            var doc = new XmlDocument();
            doc.Load(new StringReader(buffer));

            status.InnerXml = doc.InnerXml;

            status.Code = EnumReflector.ReflectEnum(doc.SelectSingleNodeValue("//status/@code"), StatusCodes.not_set);

            switch (status.Code)
            {
                case StatusCodes.invalid:

                    // there is not always an invalid child element
                    XmlNode node = doc.SelectSingleNode("//invalid");

                    if (node != null)
                    {
                        status.SubCode = EnumReflector.ReflectEnum(node.SelectAttributeValue("subcode"), StatusSubCodes.not_set);
                        status.InvalidField = node.SelectAttributeValue("field");
                        status.Type = node.SelectAttributeValue("type");
                    }

                    break;

                case StatusCodes.no_access:

                    XmlNode tNode = doc.SelectSingleNode("//status");

                    status.SubCode = EnumReflector.ReflectEnum(
                        doc.SelectSingleNodeValue("//status/@subcode"),
                        StatusSubCodes.not_set);
                    status.Type = tNode.SelectAttributeValue("type");
                    break;
            }

            return doc;
        }

        /// <summary>
        /// The process request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="HttpWebRequest"/>.
        /// </returns>
        private HttpWebRequest ProcessRequest(HttpWebRequest request)
        {
            if (request == null)
            {
                return null;
            }

            try
            {
                if (this.connectionDetails.Proxy != null && !string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Url))
                {
                    if (!string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Login)
                        && !string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Password))
                    {
                        request.Proxy = new WebProxy(this.connectionDetails.Proxy.Url, true)
                                            {
                                                Credentials = new NetworkCredential(this.connectionDetails.Proxy.Login,  this.connectionDetails.Proxy.Password, this.connectionDetails.Proxy.Domain)
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

        /// <summary>
        /// Writes multi part HTTP POST request.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="boundary">
        /// The boundary.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="fileContentType">
        /// The file Content Type.
        /// </param>
        /// <param name="fileData">
        /// The file Data.
        /// </param>
        private void WriteMultipartForm(
            Stream s, 
            string boundary, 
            Dictionary<string, string> data, 
            string fileName, 
            string fileContentType, 
            byte[] fileData)
        {
            //// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");

            //// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "–-\r\n");

            //// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";

            //// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            //// Added to track if we need a CRLF or not.
            // ReSharper disable once InconsistentNaming
            bool needsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    //// if we need to drop a CRLF, do that.
                    if (needsCRLF)
                    {
                        this.WriteToStream(s, "\r\n");
                    }

                    //// Write the boundary.
                    this.WriteToStream(s, boundarybytes);

                    //// Write the key.
                    this.WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    needsCRLF = true;
                }
            }

            //// If we don't have keys, we don't need a crlf.
            if (needsCRLF)
            {
                this.WriteToStream(s, "\r\n");
            }

            this.WriteToStream(s, boundarybytes);
            this.WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));

            //// Write the file data to the stream.
            this.WriteToStream(s, fileData);
            this.WriteToStream(s, trailer);
        }

        /// <summary>
        /// Writes string to stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        /// <param name="txt">
        /// The text.
        /// </param>
        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes byte array to stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        #endregion
    }
}