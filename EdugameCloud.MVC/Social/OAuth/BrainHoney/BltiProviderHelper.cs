﻿namespace EdugameCloud.MVC.Social.OAuth.BrainHoney
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    /// <summary>
    /// The BLTI provider helper.
    /// </summary>
    public class BltiProviderHelper
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
        /// <param name="request">
        /// Http request
        /// </param>
        /// <returns>
        /// "true" if the request is valid, otherwise "false"
        /// </returns>
        public static bool VerifyBltiRequest(HttpRequestBase request)
        {
            // First check the nonce to make sure it has not been used
            var nonce = new NonceData(request.Form["oauth_nonce"], DateTime.UtcNow);
            if (usedNonsenses.Contains(nonce))
            {
                // This nonce has already been used so the request is invalid
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
            double.TryParse(request.Form["oauth_timestamp"], out timestamp);
            double secondsSince1970 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            if (Math.Abs(secondsSince1970 - timestamp) > 5400)
            {
                // The timestamp is missing or outside of the 90 minute window so the request is invalid
                return false;
            }

            // Generate the normalized URL
            // Note that the scheme and authority must be lowercase...HttpRequestBase.Url.Scheme and HttpRequestBase.Url.Host in C# are always lowercase
            string normalizedUrl = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Host);
            if (
                !((request.Url.Scheme == "http" && request.Url.Port == 80)
                  || (request.Url.Scheme == "https" && request.Url.Port == 443)))
            {
                normalizedUrl += ":" + request.Url.Port;
            }

            normalizedUrl += request.Url.AbsolutePath;

            // Get a sorted list of all the form parameters except oauth_signature
            var parameters = new List<QueryParameter>();
            foreach (string key in request.Form.AllKeys)
            {
                if (key != "oauth_signature")
                {
                    parameters.Add(new QueryParameter(key, request.Form[key]));
                }
            }

            parameters.Sort(new QueryParameterComparer());
            string normalizedRequestParameters = parameters.NormalizeRequestParameters();

            // Create the signature base
            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", request.HttpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", normalizedUrl.OAuthUrlEncode());
            signatureBase.AppendFormat("{0}", normalizedRequestParameters.OAuthUrlEncode());

            // Look up the secret using oauth_consumer_key
            string secret = RetrieveSecretForKey(request.Form["oauth_consumer_key"]);

            var hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", secret.OAuthUrlEncode(), string.Empty /*tokenSecret not used in BLTI*/));

            byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase.ToString());
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            string signature = Convert.ToBase64String(hashBytes);

            // Check to make sure the signature matches what was passed in oauth_signature
            if (signature == request.Form["oauth_signature"])
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The retrieve secret for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If secret is not associated with the key
        /// </exception>
        private static string RetrieveSecretForKey(string key)
        {
            // Use this method to return back the secret associated with this key.
            // In this example I am using the key/secret pair { "MyKey", "Secret12345" }
            // You will need to create your own key/secret pair and replace the code below with the key/secret pair that you create.
            // The domain in BrainHoney that contains your blti links will then need to be customized with your key/secret pair.
            if (key == "MyKey")
            {
                return "Secret12345";
            }

            if (key == "MyKey2")
            {
                return "Secret123452";
            }

            throw new InvalidOperationException("No secret associated with key: " + key);
        }

        #endregion
    }
}