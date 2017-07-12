namespace EdugameCloud.Lti.Core.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The BLTI provider helper.
    /// </summary>
    public sealed class BltiProviderHelper
    {
        // THIS CODE MAY BE FREELY USED, MODIFIED, AND REDISTRIBUTED
        // ANY USE OF THIS CODE IS AT YOUR OWN RISK
        // IT IS PROVIDED “AS IS” WITHOUT WARRANTY OF ANY KIND EITHER EXPRESSED OR IMPLIED
        // UNDER NO CONDITIONS WILL THE AUTHOR OF THIS CODE BE RESPONSIBLE FOR ANY DAMAGE INCURRED BY ANY USE OF THIS CODE

        // Links to Useful Documentation
        // LTI home: http://www.imsglobal.org/lti/ 
        // BLTI Developer Support: http://www.imsglobal.org/developers/blti/
        // BLTI Forum: http://www.imsglobal.org/community/forum/categories.cfm?catid=44&entercat=y
        // BLTI implementation guide: http://www.imsglobal.org/lti/blti/bltiv1p0/ltiBLTIimgv1p0.html
        // OAuth 1.0 Protocol: http://tools.ietf.org/html/rfc5849
        // Setting up a domain with a BLTI key/secret and creating BLTI links inside of AgilixBuzz: http://may2011.brainhoney.com/docs/BasicLTI
        #region Static Fields

        private static readonly ILogger logger;
        private static readonly NonceCache usedNonsenses;
        private static readonly DateTime Date1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        static BltiProviderHelper()
        {
            logger = IoC.Resolve<ILogger>();
            usedNonsenses = new NonceCache(logger);
        }
        
        #region Public Methods and Operators

        /// <summary>
        /// This method takes a HttpRequest representing a BLTI request and checks to see if the request is valid.
        /// </summary>
        /// <param name="credentials">
        /// Credentials company
        /// </param>
        /// <param name="validateLmsCaller">
        /// Validate LMS caller function
        /// </param>
        /// <returns>
        /// "true" if the request is valid, otherwise "false"
        /// </returns>
        public static bool VerifyBltiRequest(LmsCompany credentials, HttpRequestBase request, Func<bool> validateLmsCaller)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestValues = request.Unvalidated();
            if (requestValues == null)
            {
                logger.Warn("[BltiProviderHelper] request.Unvalidated() == null");
                return false;
            }
            if (requestValues.Form == null)
            {
                logger.Warn("[BltiProviderHelper] request.Unvalidated().Form == null");
                return false;
            }

            var form = new FormCollection(requestValues.Form);
            if (form == null)
            {
                logger.Warn("[BltiProviderHelper] form == null");
                return false;
            }

            string nonce = form["oauth_nonce"];
            if (nonce == null)
            {
                logger.Warn("[BltiProviderHelper] form[oauth_nonce] == null");
                return false;
            }
            
            NonceCache.AddOrUpdateStatus nonceStatus = usedNonsenses.AddIfNotExist(nonce, () => new NonceData(nonce, DateTime.UtcNow));
            if (nonceStatus == NonceCache.AddOrUpdateStatus.Exists)
            {
                logger.WarnFormat("[BltiProviderHelper] This nonce has already been used so the request is invalid, oauth_nonce:{0}.", nonce);
                return false;
            }
            // NOTE: clean only if it is valid nonce - is it OK for us?
            usedNonsenses.TryDeleteOld();

            // Check the timestamp of the request and make sure it is within 90 minutes of the current server time
            double timestamp;
            double.TryParse(form["oauth_timestamp"], out timestamp);
            double secondsSince1970 = (DateTime.UtcNow - Date1970).TotalSeconds;
            if (Math.Abs(secondsSince1970 - timestamp) > 5400)
            {
                logger.ErrorFormat("[BltiProviderHelper] The timestamp is missing or outside of the 90 minute window so the request is invalid, oauth_timestamp:{0}.", timestamp);
                return false;
            }

            string schema = request.GetScheme();
            if (schema == null)
            {
                logger.Warn("[BltiProviderHelper] request.GetScheme() == null");
                return false;
            }
                        
            // Generate the normalized URL
            // Note that the scheme and authority must be lowercase...HttpRequestBase.Url.Scheme and HttpRequestBase.Url.Host in C# are always lowercase
            string normalizedUrl = string.Format("{0}://{1}", schema, request.Url.Host);
            if (
                !((schema == "http" && request.Url.Port == 80)
                  || (schema == "https" && request.Url.Port == 443)
                  || (schema == "https" && request.Url.Port == 80 && request.Url.Scheme == "http") // TRICK: using "X-Forwarded-Proto" proxy
                  ))
            {
                normalizedUrl += ":" + request.Url.Port;
            }

            normalizedUrl += request.Url.AbsolutePath;

            // Get a sorted list of all the form parameters except oauth_signature
            var parameters = new List<QueryParameter>();
            foreach (string key in form.AllKeys)
            {
                if (key != "oauth_signature")
                {
                    parameters.Add(new QueryParameter(key, form[key]));
                }
            }

            parameters.Sort(new QueryParameterComparer());
            string normalizedRequestParameters = parameters.NormalizeRequestParameters();

            if (normalizedUrl.OAuthUrlEncode() == null)
            {
                logger.Warn("[BltiProviderHelper] normalizedUrl.OAuthUrlEncode() == null");
                return false;
            }
            if (normalizedRequestParameters.OAuthUrlEncode() == null)
            {
                logger.Warn("[BltiProviderHelper] normalizedRequestParameters.OAuthUrlEncode() == null");
                return false;
            }

            // Create the signature base : string.Format("{0}&{1}&{2}"
            var signatureBase = string.Join("&"
                ,request.HttpMethod.ToUpper()
                ,normalizedUrl.OAuthUrlEncode().Replace("%3a80", string.Empty)
                ,normalizedRequestParameters.OAuthUrlEncode());

            string consumerKey = request["oauth_consumer_key"];
            if (string.IsNullOrWhiteSpace(consumerKey))
            {
                logger.Warn("[BltiProviderHelper] request[oauth_consumer_key] IsNullOrWhiteSpace");
                return false;
            }

            // Look up the secret using oauth_consumer_key
            string secret = RetrieveSecretForKey(consumerKey, credentials);
            if (string.IsNullOrWhiteSpace(secret))
            {
                logger.ErrorFormat("[BltiProviderHelper] Look up the secret using oauth_consumer_key failed, oauth_consumer_key:{0}.", consumerKey);
                return false;
            }

            var hmacsha1 = new HMACSHA1();
            //string.Format("{0}&{1}"
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Join("&", secret.OAuthUrlEncode(), string.Empty /*tokenSecret not used in BLTI*/));
            
            string signature = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureBase)));

            // Check to make sure the signature matches what was passed in oauth_signature
            if (signature == form["oauth_signature"])
            {
                return validateLmsCaller();
            }
            else
            {
                logger.ErrorFormat("[BltiProviderHelper] Check to make sure the signature matches what was passed in oauth_signature - failed.");
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves secret for key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="credentials">
        ///     The credentials
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RetrieveSecretForKey(string key, LmsCompany credentials)
        {
            // Use this method to return back the secret associated with this key.
            // In this example I am using the key/secret pair { "MyKey", "Secret12345" }
            // You will need to create your own key/secret pair and replace the code below with the key/secret pair that you create.
            // The domain in AgilixBuzz that contains your blti links will then need to be customized with your key/secret pair.
            if (credentials.ConsumerKey == key)
            {
                return credentials.SharedSecret;
            }

            return null;
        }

        #endregion
    }
}