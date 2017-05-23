using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Esynctraining.AC.Provider.Utils
{
    using System;
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
    /// The request processor.
    /// </summary>
    internal class RequestProcessor
    {
        #region Fields

        private readonly ConnectionDetails connectionDetails;
        private Cookie sessionCookie;
        private Cookie breezeCCookie;

        #endregion

        #region Constructors and Destructors

        public RequestProcessor(ConnectionDetails details)
        {
            connectionDetails = details ?? throw new ArgumentNullException(nameof(details));
            SetSessionId(null);
        }

        #endregion

        #region Public Properties

        public Uri AdobeConnectRoot
        {
            get { return connectionDetails.AdobeConnectRoot; }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether is logged in.
        /// </summary>
        private bool IsLoggedIn
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
        private bool IsSessionCookieValid
        {
            get
            {
                return this.sessionCookie != null
                    && !string.IsNullOrWhiteSpace(this.sessionCookie.Value)
                    && !string.IsNullOrWhiteSpace(this.sessionCookie.Domain);
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task<CreatingEventResponse> GetAcAdminResponseRedirectLocation(string sharedEventsFolderScoId, string owasp)
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(sessionCookie);
            cookieContainer.Add(breezeCCookie);
            // get response redirect location
            var httpMessageHandler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = false };
            using (var handler = httpMessageHandler)
            {
                using (var client = new HttpClient(handler))
                {
                    var getTask = await client.GetAsync(connectionDetails.AdobeConnectRoot.AbsoluteUri +
                                                 $"admin/event/folder/list/new?filter-rows=100&filter-start=0&parent-acl-id={sharedEventsFolderScoId}&sco-id={sharedEventsFolderScoId}&start-id={sharedEventsFolderScoId}&tab-id={sharedEventsFolderScoId}&OWASP_CSRFTOKEN={owasp}");
                    var result = getTask;
                    
                    var location = result.Headers.Location;

                    var queryStringDict = location.DecodeQueryParameters();
                    var eventScoId = queryStringDict["sco-id"];
                    
                    if (string.IsNullOrEmpty(eventScoId))
                        throw new InvalidOperationException("ScoId of newly created event can't be empty!");

                    string eventTemplateId = String.Empty;
                    using (var innerClient = new HttpClient(httpMessageHandler))
                    {
                        var innerResult = innerClient.GetAsync(result.Headers.Location).Result;
                        var returnedHtmlPage = innerResult.Content.ReadAsStringAsync().Result;
                        //var text = @"<?xml version=""1.0"" encoding=""utf-8""?>" + @"<html><body></body></html>";
                        var text = @"<?xml version=""1.0"" encoding=""utf-8""?>" + returnedHtmlPage.Trim();
                        //var xml = new XmlDocument();
                        //xml.PreserveWhitespace = false;
                        //xml.LoadXml(text);
                        //var res1 = xml.SelectSingleNode("select");
                        //var xml = XDocument.Parse(text, LoadOptions.PreserveWhitespace);
                        //var desc = xml.Root?.Descendants("select");
                        //var cqTemplate = xml.Root?.Descendants("select").Single(x => x.Attribute("id")?.Value == "cqTemplate");
                        //var option = cqTemplate?.Descendants("option").Single(x => x.Value == "Shared-Default Template");
                        //eventTemplateId = option?.Attribute("value")?.Value;
                        var pattern =
                            //@"value=""(\d+)"" id=""javascript: load\('\/system\/login-redirect\?next='\+encodeURIComponent\('cq-auth:\/content\/connect\/c1\/7\/en\/events\/event\/shared\/default_template\/event_landing\.html'\)\)";
                            @"value=""(\d+)"" id=""javascript:load\('\/system\/login-redirect\?next='\+encodeURIComponent\('cq-auth:\/content\/connect\/c1\/7\/en\/events\/event\/shared\/default_template\/event_landing\.html'\)\)";
                            //@"value=""(\d+)""";
                        var regex = new Regex(pattern);
                        var matches = regex.Matches(text);
                        var match = matches[0];
                        var group = match.Groups[1].Value;
                        eventTemplateId = group;
                    }

                    var res = new CreatingEventResponse()
                    {
                        CreateEventPostUrl = location,
                        ScoId = eventScoId,
                        EventTemplateId = eventTemplateId
                    };
                    return res;
                }
            }
        }

        public LoginAsOnUiContainer LoginAsOnUi(UserCredentials adminUser)
        {
            var login = adminUser.Login;
            var acUrl = connectionDetails.AdobeConnectRoot.AbsoluteUri;
            var pass = adminUser.Password;

            var breezeSession = "";
            var breezeCCookieResult = "";
            var owasp = "";
            using (var handler = new HttpClientHandler() { AllowAutoRedirect = false })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(acUrl) })
                {
                    var path =
                        $"system/login/ok?domain={ WebUtility.UrlEncode(acUrl)}&next=%2F&set-lang=en";
                    var result =
                        client.PostAsync(path, new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("login", login),
                            new KeyValuePair<string, string>("password", pass),
                        })).Result;

                    IEnumerable<string> cookieValues;
                    result.Headers.TryGetValues("Set-Cookie", out cookieValues);
                    var breezeSessionFull = cookieValues.First().Split(';')[0];
                    var parsedSessionCookie =
                        breezeSessionFull.Substring(breezeSessionFull.IndexOf("=", StringComparison.Ordinal) + 1);
                    breezeSession = parsedSessionCookie;

                    using (var innerClient = new HttpClient())
                    {
                        var innerResult = innerClient.GetAsync(result.Headers.Location).Result;
                        innerResult.Headers.TryGetValues("Set-Cookie", out cookieValues);
                        var breezeCCookieFull = cookieValues.First().Split(';')[0];
                        var cookie =
                            breezeCCookieFull.Substring(breezeCCookieFull.IndexOf("=", StringComparison.Ordinal) + 1);

                        var location = innerResult.RequestMessage.RequestUri;
                        var queryStringDict = location.DecodeQueryParameters();

                        owasp = queryStringDict["OWASP_CSRFTOKEN"];
                        breezeCCookieResult = cookie;
                    }
                }
            }

            //breezeSession = "breezbreez4q42qbe9fwk67eq8";
            //breezeCCookieResult = "P8FO-IME7-HQNH-U4G1-1YVB-YC08-FVKS-SG4Z";
            //owasp = "3953ed4a8da0089576679d254aa07a305ff707f70aedc164c3dac3ec7da3fbb6";
            SetSessionId(breezeSession);
            SetBreezeCCookie(breezeCCookieResult);
            return new LoginAsOnUiContainer()
            {
                Owasp = owasp,
                BreezeSession = breezeSession,
                BreezeCCookie = breezeCCookieResult
            };
        }

        public StatusInfo PostAcAdminRequest(CreatingEventContainer settings)
        {
            var baseAddress = new Uri(connectionDetails.AdobeConnectRoot.AbsoluteUri);
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(baseAddress, new Cookie(sessionCookie.Name, sessionCookie.Value));
            cookieContainer.Add(baseAddress, new Cookie(breezeCCookie.Name, breezeCCookie.Value));
            var messageHandler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = true };
            using (var handler = messageHandler)
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var requestUri = $"admin/event/folder/list/new/1/next?account-id={settings.AccountId}&filter-rows=100&filter-start=0&sco-id={settings.EventScoId}&start-id={settings.SharedEventsFolderScoId}&tab-id={settings.SharedEventsFolderScoId}&OWASP_CSRFTOKEN={settings.Owasp}";
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Host", baseAddress.DnsSafeHost);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Origin", baseAddress.AbsoluteUri);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", requestUri);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en,en-US;q=0.8,ru;q=0.6");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    var content = new MultipartFormDataContent();
                    foreach (var eventProperty in settings.EventProperties)
                    {
                        var stringContent = new StringContent(eventProperty.Value);
                        stringContent.Headers.Remove("Content-Type");
                        content.Add(stringContent, eventProperty.Key);
                    }

                    var result = client.PostAsync(requestUri, content).Result;
                    var status = new StatusInfo {SessionInfo = sessionCookie.Value};
                    return status;
                }
            }
        }

        public byte[] DownloadData(string urlPath, string format, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/output/{1}.{2}?download={2}",
                connectionDetails.AdobeConnectRoot.ToString().TrimEnd('/'),
                urlPath.Trim('/'),
                format);

            return DownloadData(url, out error);
        }

        public byte[] DownloadData2(string urlPath, string fileName, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/output/{2}?download={2}",
                connectionDetails.AdobeConnectRoot.ToString().TrimEnd('/'),
                urlPath.Trim('/'),
                fileName);

            return DownloadData(url, out error);
        }

        public byte[] DownloadSourceData2(string urlPath, string format, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/source/{1}.{2}?download={2}",
                connectionDetails.AdobeConnectRoot.ToString().TrimEnd('/'),
                urlPath.Trim('/'),
                format);

            return DownloadData(url, out error);
        }

        public byte[] DownloadSourceData(string urlPath, string fileName, out string error)
        {
            error = null;
            string url = string.Format(
                "{0}/{1}/source/{2}?download={2}",
                connectionDetails.AdobeConnectRoot.ToString().TrimEnd('/'),
                urlPath.Trim('/'),
                fileName);

            return DownloadData(url, out error);
        }

        private byte[] DownloadData(string url, out string error)
        {
            error = null;
            var request = WebRequest.Create(url) as HttpWebRequest;
            request = ProcessRequest(request, contentRequest: true);

            // TRICK: Empty value causes 500 error for 'http://connect.uthsc.edu/' during File Download
            request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";

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

            HttpWebRequest webRequest = CreateWebRequest(action, parameters);

            if (webRequest == null)
            {
                return null;
            }

            return this.FinalizeProcessingResponse(status, webRequest);
        }

        public XmlDocument ProcessUpload(
            string action,
            string parameters,
            UploadScoInfo uploadData,
            out StatusInfo status)
        {
            status = new StatusInfo();

            if (parameters == null)
            {
                parameters = string.Empty;
            }

            try
            {
                HttpResponseMessage response = null;
                var url = BuildUrl(action, parameters) + $"&session={sessionCookie.Value}";
                // ReSharper disable once UseObjectOrCollectionInitializer
                var form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(uploadData.fileBytes, 0, uploadData.fileBytes.Length), "file",
                    uploadData.fileName);
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(connectionDetails.HttpContentRequestTimeout);

                    response = httpClient.PostAsync(url, form).Result;
                }

                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                return ProcessXmlResult(status, result);
            }
            catch (Exception ex)
            {
                status.UnderlyingExceptionInfo = ex;
                TraceTool.TraceException(ex);
                return null;
            }
        }

        //public XmlDocument ProcessUploadMultipart(
        //    string action, 
        //    string parameters, 
        //    UploadScoInfo uploadData, 
        //    out StatusInfo status)
        //{
        //    status = new StatusInfo { Code = StatusCodes.not_set };

        //    if (parameters == null)
        //    {
        //        parameters = string.Empty;
        //    }

        //    HttpWebRequest webRequest = this.CreateWebRequest(action, parameters);

        //    if (webRequest == null || uploadData == null)
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
        //        webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
        //        webRequest.Method = "POST";
        //        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //        using (Stream requestStream = webRequest.GetRequestStream())
        //        {
        //            WriteMultipartForm(
        //                requestStream, 
        //                boundary, 
        //                new Dictionary<string, string>(), 
        //                uploadData.fileName, 
        //                uploadData.fileContentType, 
        //                uploadData.fileBytes);
        //        }

        //        return this.FinalizeProcessingResponse(status, webRequest);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceTool.TraceException(ex);
        //        return null;
        //    }
        //}

        public void SetSessionId(string sessionId)
        {
            this.sessionCookie = new Cookie(
                AdobeConnectProviderConstants.SessionCookieName,
                sessionId,
                "/",
                connectionDetails.AdobeConnectRoot.Host);
        }

        public void SetBreezeCCookie(string sessionId)
        {
            this.breezeCCookie = new Cookie(
                AdobeConnectProviderConstants.BreezeCCookie,
                sessionId,
                "/",
                connectionDetails.AdobeConnectRoot.Host);
        }

        #endregion

        #region Methods

        private string BuildUrl(string action, string parameters)
        {
            var url = $"{connectionDetails.AdobeConnectRoot.ToString().TrimEnd('/')}/api/xml?action={action}&{parameters}";
            return url;
        }

        protected HttpWebRequest CreateWebRequest(string action, string parameters)
        {
            //this.connectionDetails.AdobeConnectRoot + string.Format(@"api/xml?action={0}&{1}", action, parameters)
            var url = BuildUrl(action, parameters);
            var request = WebRequest.Create(url) as HttpWebRequest;
            return ProcessRequest(request);
        }

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

        protected XmlDocument ProcessXmlResult(StatusInfo status, string buffer)
        {
            var doc = new XmlDocument();

            try
            {
                doc.Load(new StringReader(buffer));
            }
            catch (XmlException ex)
            {
                doc.Load(new StringReader(RemoveTroublesomeCharacters(buffer)));
            }
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

        // https://stackoverflow.com/questions/20762/how-do-you-remove-invalid-hexadecimal-characters-from-an-xml-based-data-source-p?rq=1
        private static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null)
                return null;

            var newString = new StringBuilder(inString.Length);
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                //if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                //if using .NET version prior to 4, use above logic
                if (XmlConvert.IsXmlChar(ch)) //this method is new in .NET 4
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }

        private HttpWebRequest ProcessRequest(HttpWebRequest request, bool contentRequest = false)
        {
            //try
            //{
            //    //if (connectionDetails.Proxy != null && !string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Url))
            //    //{
            //    //    if (!string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Login)
            //    //        && !string.IsNullOrWhiteSpace(this.connectionDetails.Proxy.Password))
            //    //    {
            //    //        request.Proxy = new WebProxy(this.connectionDetails.Proxy.Url, true)
            //    //        {
            //    //            Credentials = new NetworkCredential(this.connectionDetails.Proxy.Login,  this.connectionDetails.Proxy.Password, this.connectionDetails.Proxy.Domain)
            //    //        };
            //    //    }
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    TraceTool.TraceException(ex);
            //}

            // 20 sec. timeout: A Domain Name System (DNS) query may take up to 15 seconds to return or time out.
            request.Timeout = contentRequest ? connectionDetails.HttpContentRequestTimeout : connectionDetails.HttpRequestTimeout;
            request.Accept = "*/*";
            request.KeepAlive = false;
            request.CookieContainer = new CookieContainer();
            //// Empty value causes 500 error for 'http://connect.uthsc.edu/' during File Download
            //request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";

            if (this.IsLoggedIn)
            {
                request.CookieContainer.Add(this.sessionCookie);
            }

            return request;
        }

        //private static void WriteMultipartForm(
        //    Stream s, 
        //    string boundary, 
        //    Dictionary<string, string> data, 
        //    string fileName, 
        //    string fileContentType, 
        //    byte[] fileData)
        //{
        //    //// The first boundary
        //    byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");

        //    //// the last boundary.
        //    byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "–-\r\n");

        //    //// the form data, properly formatted
        //    string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";

        //    //// the form-data file upload, properly formatted
        //    string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

        //    //// Added to track if we need a CRLF or not.
        //    // ReSharper disable once InconsistentNaming
        //    bool needsCRLF = false;

        //    if (data != null)
        //    {
        //        foreach (string key in data.Keys)
        //        {
        //            //// if we need to drop a CRLF, do that.
        //            if (needsCRLF)
        //            {
        //                WriteToStream(s, "\r\n");
        //            }

        //            //// Write the boundary.
        //            WriteToStream(s, boundarybytes);

        //            //// Write the key.
        //            WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
        //            needsCRLF = true;
        //        }
        //    }

        //    //// If we don't have keys, we don't need a crlf.
        //    if (needsCRLF)
        //    {
        //        WriteToStream(s, "\r\n");
        //    }

        //    WriteToStream(s, boundarybytes);
        //    WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));

        //    //// Write the file data to the stream.
        //    WriteToStream(s, fileData);
        //    WriteToStream(s, trailer);
        //}

        //private static void WriteToStream(Stream s, string txt)
        //{
        //    byte[] bytes = Encoding.UTF8.GetBytes(txt);
        //    s.Write(bytes, 0, bytes.Length);
        //}

        //private static void WriteToStream(Stream s, byte[] bytes)
        //{
        //    s.Write(bytes, 0, bytes.Length);
        //}

        #endregion

    }

}