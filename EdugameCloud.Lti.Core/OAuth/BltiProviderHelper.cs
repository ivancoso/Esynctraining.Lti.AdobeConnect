namespace EdugameCloud.Lti.Core.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.Extensions;
    using Castle.Core.Logging;
    using System.Web.Helpers;
    using System.Web.Mvc;

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
        // Setting up a domain with a BLTI key/secret and creating BLTI links inside of BrainHoney: http://may2011.brainhoney.com/docs/BasicLTI
        #region Static Fields

        /// <summary>
        /// The used nonsense.
        /// </summary>
        private static readonly List<NonceData> usedNonsenses = new List<NonceData>();

        #endregion

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
        public static bool VerifyBltiRequest(LmsCompany credentials, ILogger logger, Func<bool> validateLmsCaller)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (logger == null)
                throw new ArgumentNullException("logger");

            var request = HttpContext.Current.Request;
            FormCollection form = new FormCollection(request.Unvalidated().Form);
            // First check the nonce to make sure it has not been used
            var nonce = new NonceData(form["oauth_nonce"], DateTime.UtcNow);
            if (usedNonsenses.Contains(nonce))
            {
                logger.WarnFormat("[BltiProviderHelper] This nonce has already been used so the request is invalid, oauth_nonce:{0}.", form["oauth_nonce"]);
                return false;
            }

            // Add this nonce to the list of used nonces
            usedNonsenses.Add(nonce);

            // Do housekeeping on the list of nonces. 
            // The BLTI spec recommends only keeping a record of nonces used within the last 90 minutes
            for (int i = usedNonsenses.Count - 1; i >= 0; i--)
            {
                if (DateTime.UtcNow.Subtract(usedNonsenses[i].Timestamp).TotalMinutes > 90)
                {
                    usedNonsenses.RemoveAt(i);
                }
            }

            // Check the timestamp of the request and make sure it is within 90 minutes of the current server time
            double timestamp;
            double.TryParse(form["oauth_timestamp"], out timestamp);
            double secondsSince1970 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            if (Math.Abs(secondsSince1970 - timestamp) > 5400)
            {
                logger.ErrorFormat("[BltiProviderHelper] The timestamp is missing or outside of the 90 minute window so the request is invalid, oauth_timestamp:{0}.", timestamp);
                return false;
            }

            string schema = request.GetScheme();
            
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

            // Create the signature base
            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&{1}&{2}", 
                request.HttpMethod.ToUpper()
                ,normalizedUrl.OAuthUrlEncode().Replace("%3a80", string.Empty)
                ,normalizedRequestParameters.OAuthUrlEncode());

            // Look up the secret using oauth_consumer_key
            string secret = RetrieveSecretForKey(request["oauth_consumer_key"], credentials);

            if (string.IsNullOrWhiteSpace(secret))
            {
                logger.ErrorFormat("[BltiProviderHelper] Look up the secret using oauth_consumer_key failed, oauth_consumer_key:{0}.", request["oauth_consumer_key"]);
                return false;
            }

            var hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", secret.OAuthUrlEncode(), string.Empty /*tokenSecret not used in BLTI*/));

            byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase.ToString());
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            string signature = Convert.ToBase64String(hashBytes);

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
        /// The retrieve secret for key.
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
        /// <exception cref="InvalidOperationException">
        /// If secret is not associated with the key
        /// </exception>
        private static string RetrieveSecretForKey(string key, LmsCompany credentials)
        {
            // Use this method to return back the secret associated with this key.
            // In this example I am using the key/secret pair { "MyKey", "Secret12345" }
            // You will need to create your own key/secret pair and replace the code below with the key/secret pair that you create.
            // The domain in BrainHoney that contains your blti links will then need to be customized with your key/secret pair.
            if (credentials.ConsumerKey == key)
            {
                return credentials.SharedSecret;
            }

            return null;
        }

        #endregion
    }
}