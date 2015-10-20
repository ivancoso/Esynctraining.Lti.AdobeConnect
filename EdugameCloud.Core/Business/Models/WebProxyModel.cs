//namespace EdugameCloud.Core.Business.Models
//{
//    using System;
//    using System.Globalization;
//    using System.IO;
//    using System.Linq;
//    using System.Net;
//    using System.Text;
//    using Esynctraining.Core.Logging;
//    using EdugameCloud.Core.Domain.DTO;

//    using Esynctraining.Core.Providers;

//    using RestSharp;

//    /// <summary>
//    /// The web proxy model.
//    /// </summary>
//    public class WebProxyModel
//    {
//        #region Fields

//        /// <summary>
//        ///     The logger.
//        /// </summary>
//        private readonly ILogger logger;

//        /// <summary>
//        ///     The settings.
//        /// </summary>
//        private readonly dynamic settings;

//        #endregion

//        #region Constructors and Destructors

//        /// <summary>
//        /// Initializes a new instance of the <see cref="WebProxyModel"/> class.
//        /// </summary>
//        /// <param name="settings">
//        /// The settings.
//        /// </param>
//        /// <param name="logger">
//        /// The logger.
//        /// </param>
//        public WebProxyModel(ApplicationSettingsProvider settings, ILogger logger)
//        {
//            this.settings = settings;
//            this.logger = logger;
//        }

//        #endregion

//        #region Public Methods and Operators

//        /// <summary>
//        /// The unsubscribe INSTAGRAM tag.
//        /// </summary>
//        /// <param name="tag">
//        /// The tag.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionResDTO"/>.
//        /// </returns>
//        public SubscriptionResDTO UnsubscribeInstagramTag(string tag)
//        {
//            var all = this.ListSubscriptions();
//            var tagSubscription = all.data.FirstOrDefault(x => x.object_id.Equals(tag) && x.@object == "tag");
//            return tagSubscription != null
//                       ? this.DeleteInstagramSubscription(tagSubscription.id)
//                       : new SubscriptionResDTO
//                             {
//                                 meta = new SubscriptionMeta { code = 404 },
//                                 raw = "Subscription not found"
//                             };
//        }

//        /// <summary>
//        /// The delete INSTAGRAM subscription.
//        /// </summary>
//        /// <param name="id">
//        /// The id.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionResDTO"/>.
//        /// </returns>
//        public SubscriptionResDTO DeleteAllInstagramSubscriptions()
//        {
//            var client_id = (string)this.settings.InstagramClientId;
//            var client_secret = (string)this.settings.InstagramClientSecret;
//            var client = new RestClient("https://api.instagram.com");
//            var request = new RestRequest("v1/subscriptions?client_secret={client_secret}&client_id={client_id}&id={subscription_id}&object={object_type}", Method.DELETE);
//            request.AddUrlSegment("client_id", client_id);
//            request.AddUrlSegment("client_secret", client_secret);
//            request.AddUrlSegment("object_type", "all");
//            try
//            {
//                var response = client.Execute<SubscriptionResDTO>(request);
//                var res = response.Data;
//                res.raw = response.Content;
//                return res;
//            }
//            catch (Exception)
//            {
//                var response = client.Execute(request);
//                return new SubscriptionResDTO { raw = response.Content };
//            }
//        }

//        /// <summary>
//        /// The delete INSTAGRAM subscription.
//        /// </summary>
//        /// <param name="id">
//        /// The id.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionResDTO"/>.
//        /// </returns>
//        public SubscriptionResDTO DeleteInstagramSubscription(int id)
//        {
//            var client_id = (string)this.settings.InstagramClientId;
//            var client_secret = (string)this.settings.InstagramClientSecret;
//            var client = new RestClient("https://api.instagram.com");
//            var request = new RestRequest("v1/subscriptions?client_secret={client_secret}&client_id={client_id}&id={subscription_id}", Method.DELETE);
//            request.AddUrlSegment("client_id", client_id);
//            request.AddUrlSegment("client_secret", client_secret);
//            request.AddUrlSegment("subscription_id", id.ToString(CultureInfo.InvariantCulture));
//            try
//            {
//                var response = client.Execute<SubscriptionResDTO>(request);
//                var res = response.Data;
//                res.raw = response.Content;
//                return res;
//            }
//            catch (Exception)
//            {
//                var response = client.Execute(request);
//                return new SubscriptionResDTO { raw = response.Content };
//            }
//        }

//        /// <summary>
//        /// The subscribe to INSTAGRAM tag.
//        /// </summary>
//        /// <param name="tag">
//        /// The tag.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionResDTO"/>.
//        /// </returns>
//        public SubscriptionResDTO SubscribeToInstagramTag(string tag)
//        {
//            var client_id = (string)this.settings.InstagramClientId;
//            var client_secret = (string)this.settings.InstagramClientSecret;
//            var callbackurl = ((string)this.settings.BasePath).Replace(@"services/", string.Empty) + "social/instagram-realtime";
//            var client = new RestClient("https://api.instagram.com");
//            var request = new RestRequest("v1/subscriptions", Method.POST);
//            request.AddParameter("client_id", client_id);
//            request.AddParameter("client_secret", client_secret);
//            request.AddParameter("object", "tag");
//            request.AddParameter("aspect", "media");
//            request.AddParameter("verify_token", Guid.NewGuid().ToString());
//            request.AddParameter("object_id", tag);
//            request.AddParameter("callback_url", callbackurl);
//            try
//            {
//                var response = client.Execute<SubscriptionResDTO>(request);
//                var res = response.Data;
//                res.raw = response.Content;
//                return res;
//            }
//            catch (Exception)
//            {
//                var response = client.Execute(request);
//                return new SubscriptionResDTO { raw = response.Content };
//            }
//        }

//        public SubscriptionDTO ListSubscriptions()
//        {
//            var client_id = (string)this.settings.InstagramClientId;
//            var client_secret = (string)this.settings.InstagramClientSecret;
//            var client = new RestClient("https://api.instagram.com");
//            var request = new RestRequest("v1/subscriptions?client_secret={client_secret}&client_id={client_id}", Method.GET);
//            request.AddUrlSegment("client_id", client_id);
//            request.AddUrlSegment("client_secret", client_secret);
            
//            var response = client.Execute<SubscriptionDTO>(request);
//            return response.Data;
//        }

//        /// <summary>
//        /// The get.
//        /// </summary>
//        /// <param name="webParams">
//        /// The web parameters.
//        /// </param>
//        /// <returns>
//        /// The <see cref="string"/>.
//        /// </returns>
//        public string Get(WebRequestDTO webParams, out bool result)
//        {
//            string strResponse;
//            try
//            {
//                var req = (HttpWebRequest)WebRequest.Create(webParams.url);
//                req.Method = "GET";
//                if (!string.IsNullOrWhiteSpace(webParams.contentType))
//                {
//                    req.ContentType = webParams.contentType;
//                }

//                if (webParams.headers != null && webParams.headers.Any())
//                {
//                    foreach (var header in webParams.headers)
//                    {
//                        req.Headers.Add(header.name, header.value);
//                    }
//                }

//                var res = req.GetResponse();

//                var inputStream = res.GetResponseStream();
//                if (inputStream != null)
//                {
//                    using (var inputStreamReader = new StreamReader(inputStream))
//                    {
//                        strResponse = inputStreamReader.ReadToEnd();
//                        inputStreamReader.Close();
//                    }
                    
//                }
//                else
//                {
//                    strResponse = string.Empty;
//                }

//                result = true;
//            }
//            catch (Exception ex)
//            {
//                result = false;
//                strResponse = ex.ToString();
//                this.logger.Error("Post error: ", ex);
//            }

//            return strResponse;
//        }

//        /// <summary>
//        /// The post.
//        /// </summary>
//        /// <param name="webParams">
//        /// The web parameters.
//        /// </param>
//        /// <param name="result">
//        /// The result.
//        /// </param>
//        /// <returns>
//        /// The <see cref="string"/>.
//        /// </returns>
//        public string Post(WebRequestDTO webParams, out bool result)
//        {
//            string strResponse = string.Empty;
//            try
//            {
//                var req = (HttpWebRequest)WebRequest.Create(webParams.url);
//                req.KeepAlive = false;
//                req.Method = "POST";
//                req.ContentType = string.IsNullOrWhiteSpace(webParams.contentType)
//                                      ? "application/json"
//                                      : webParams.contentType;
//                req.ContentLength = webParams.data.Length;
//                if (webParams.headers != null && webParams.headers.Any())
//                {
//                    foreach (var header in webParams.headers)
//                    {
//                        req.Headers.Add(header.name, header.value);
//                    }
//                }

//                using (var outStream = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
//                {
//                    outStream.Write(webParams.data);
//                    outStream.Close();

//                    var res = req.GetResponse();
//                    var inputStream = res.GetResponseStream();
//                    if (inputStream != null)
//                    {
//                        using (var inputStreamReader = new StreamReader(inputStream))
//                        {
//                            strResponse = inputStreamReader.ReadToEnd();
//                            inputStreamReader.Close();
//                        }
//                    }
//                    else
//                    {
//                        strResponse = string.Empty;
//                    }
//                }

//                result = true;
//            }
//            catch (WebException ex)
//            {
//                result = false;
//                try
//                {
//                    var inputStream = ex.Response.GetResponseStream();
//                    if (inputStream != null)
//                    {
//                        using (var inputStreamReader = new StreamReader(inputStream))
//                        {
//                            strResponse = inputStreamReader.ReadToEnd();
//                            inputStreamReader.Close();
//                        }
//                    }
//                    else
//                    {
//                        strResponse = string.Empty;
//                    }

//                    this.logger.Error("Post web error: " + strResponse);
//                }
//                catch (Exception ex2)
//                {
//                    this.logger.Error("Post web error: ", ex2);
//                }
//            }
//            catch (Exception ex)
//            {
//                result = false;
//                strResponse = ex.ToString();
//                this.logger.Error("Post error: ", ex);
//            }

//            return strResponse;
//        }

//        #endregion
//    }
//}
