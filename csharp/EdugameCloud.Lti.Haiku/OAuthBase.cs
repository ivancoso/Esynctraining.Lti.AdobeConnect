﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuOAuthBaseClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HaikuOAuthBaseClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1,
        }

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            public string Name { get; }

            public string Value { get; }


            public QueryParameter(string name, string value)
            {
                Name = name;
                Value = value;
            }

        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {

            #region IComparer<QueryParameter> Members

            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }

        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //		
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";
        protected const string OAuthVerifierKey = "oauth_verifier";

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        protected Random random = new Random();

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public string UrlEncode(string value)
        {
            var result = new StringBuilder(value.Length);

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + string.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="callBackUrl">The callback URL (for OAuth 1.0a).If your client cannot accept callbacks, the value MUST be 'oob' </param>
        /// <param name="oauthVerifier">This value MUST be included when exchanging Request Tokens for Access Tokens. Otherwise pass a null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="HaikuOAuthBaseClient.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base</returns>
        public string GenerateSignatureBase(Uri url, string callBackUrl, string oauthVerifier, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            if (Token == null)
                Token = string.Empty;
            if (TokenSecret == null)
                TokenSecret = string.Empty;

            if (string.IsNullOrEmpty(httpMethod))
                throw new ArgumentNullException(nameof(httpMethod));
            if (string.IsNullOrEmpty(signatureType))
                throw new ArgumentNullException(nameof(signatureType));

            if (string.IsNullOrEmpty(ConsumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            normalizedUrl = null;
            normalizedRequestParameters = null;

            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, this.ConsumerKey));

            if (!string.IsNullOrEmpty(callBackUrl))
            {
                parameters.Add(new QueryParameter(OAuthCallbackKey, UrlEncode(callBackUrl)));
            }


            if (!string.IsNullOrEmpty(oauthVerifier))
            {
                parameters.Add(new QueryParameter(OAuthVerifierKey, oauthVerifier));
            }

            if (!string.IsNullOrEmpty(this.Token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, Token));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>	
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="callBackUrl">The callback URL (for OAuth 1.0a).If your client cannot accept callbacks, the value MUST be 'oob' </param>
        /// <param name="oauthVerifier">This value MUST be included when exchanging Request Tokens for Access Tokens. Otherwise pass a null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string callBackUrl, string oauthVerifier, string httpMethod, string timeStamp, string nonce, out string normalizedUrl, out string normalizedRequestParameters)
        {
            return GenerateSignature(url, callBackUrl, oauthVerifier, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>	
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="callBackUrl">The callback URL (for OAuth 1.0a).If your client cannot accept callbacks, the value MUST be 'oob' </param>
        /// <param name="oauthVerifier">This value MUST be included when exchanging Request Tokens for Access Tokens. Otherwise pass a null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string callBackUrl, string oauthVerifier, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", this.ConsumerSecret, this.TokenSecret));
                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(url, callBackUrl, oauthVerifier, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, out normalizedUrl, out normalizedRequestParameters);

                    var hmacsha1 = new HMACSHA1
                    {
                        Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(ConsumerSecret), string.IsNullOrEmpty(TokenSecret) ? string.Empty : UrlEncode(TokenSecret)))
                    };

                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", nameof(signatureType));
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature		
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next(123400, 9999999).ToString();
        }

        public enum Method { GET, POST, PUT, DELETE };

        public string ConsumerKey { get; set; } = "asdf";
        public string ConsumerSecret { get; set; } = "ghjk";
        public string Token { get; set; } = "";
        public string TokenSecret { get; set; } = "";
        public string CallBackUrl { get; set; } = "oob";
        public string OAuthVerifier { get; set; } = "";


        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public async Task<string> oAuthWebRequestAsync(Method method, string url, string postData)
        {
            if (method != Method.GET)
            {
                throw new NotImplementedException();
            }

            string outUrl = "";
            string querystring = "";
            string ret = "";
            
            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == Method.POST || method == Method.DELETE)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = this.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];

                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            var uri = new Uri(url);

            string nonce = GenerateNonce();
            string timeStamp = GenerateTimeStamp();

            //Generate Signature
            string sig = GenerateSignature(uri,
                CallBackUrl,
                OAuthVerifier,
                method.ToString(),
                timeStamp,
                nonce,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + UrlEncode(sig);

            //Convert the querystring to postData
            if (method == Method.POST || method == Method.DELETE)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(outUrl + querystring);
            return await response.Content.ReadAsStringAsync();
        }

        ///// <summary>
        ///// Web Request Wrapper
        ///// </summary>
        ///// <param name="method">Http Method</param>
        ///// <param name="url">Full url to the web resource</param>
        ///// <param name="postData">Data to post in querystring format</param>
        ///// <returns>The web server response.</returns>
        //private string WebRequest(Method method, string url, string postData)
        //{
        //    HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
        //    webRequest.Method = method.ToString();
        //    webRequest.ServicePoint.Expect100Continue = false;
        //    //webRequest.UserAgent	= "Identify your application please.";
        //    //webRequest.Timeout = 20000;

        //    if (method == Method.POST || method == Method.DELETE)
        //    {
        //        webRequest.ContentType = "application/x-www-form-urlencoded";

        //        //POST the data.
        //        StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
        //        try
        //        {
        //            requestWriter.Write(postData);
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            requestWriter.Close();
        //            requestWriter = null;
        //        }
        //    }

        //    string responseData = "";

        //    responseData = WebResponseGet(webRequest);

        //    webRequest = null;

        //    return responseData;

        //}

        ///// <summary>
        ///// Process the web response.
        ///// </summary>
        ///// <param name="webRequest">The request object.</param>
        ///// <returns>The response data.</returns>
        //private string WebResponseGet(HttpWebRequest webRequest)
        //{
        //    StreamReader responseReader = null;
        //    string responseData = "";

        //    try
        //    {
        //        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
        //        responseData = responseReader.ReadToEnd();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        webRequest.GetResponse().GetResponseStream().Close();
        //        responseReader.Close();
        //        responseReader = null;
        //    }

        //    return responseData;
        //}

    }

}
