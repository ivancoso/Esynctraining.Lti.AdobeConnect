namespace Esynctraining.AC.Provider.Utils
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    using Esynctraining.AC.Provider.Constants;
    using Esynctraining.AC.Provider.DataObjects;
    using Esynctraining.AC.Provider.Entities;

    internal class RequestProcessorAsync : RequestProcessor
    {
        public RequestProcessorAsync(ConnectionDetails details) : base(details)
        {
        }
        

        public async Task<Tuple<XmlDocument, StatusInfo>> Process(string action, string parameters)
        {
            if (parameters == null)
                parameters = string.Empty;

            HttpWebRequest webRequest = CreateWebRequest(action, parameters);

            if (webRequest == null)
                return null;

            var status = new StatusInfo { Code = StatusCodes.not_set };
            var xml = await FinalizeProcessingResponse(status, webRequest);
            return new Tuple<XmlDocument, StatusInfo>(xml, status);
        }
        

        private async Task<XmlDocument> FinalizeProcessingResponse(StatusInfo status, HttpWebRequest webRequest)
        {
            HttpWebResponse webResponse = null;

            try
            {
                // FIX: invalid SSL passing behavior
                // (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                webResponse = await webRequest.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;

                if (webResponse == null)
                {
                    return null;
                }

                if (!this.IsLoggedIn && (webResponse.Cookies[AdobeConnectProviderConstants.SessionCookieName] != null))
                {
                    this.sessionCookie = webResponse.Cookies[AdobeConnectProviderConstants.SessionCookieName];
                    status.SessionInfo = this.sessionCookie.Value;
                }

                Stream receiveStream = webResponse.GetResponseStream();

                if (receiveStream == null)
                {
                    return null;
                }

                XmlDocument doc = null;

                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    string buf = await readStream.ReadToEndAsync();

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

    }

}