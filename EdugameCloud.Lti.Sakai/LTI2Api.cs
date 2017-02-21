﻿using System.Linq;

namespace EdugameCloud.Lti.Sakai
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using API;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;

    /// <summary>
    /// The LTI 2 API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class LTI2Api : ILmsAPI
    {
        #region Constants

        /// <summary>
        /// The OAUTH signature method.
        /// </summary>
        private const string OAuthSignatureMethod = "HMAC-SHA1";

        /// <summary>
        /// The OAUTH version.
        /// </summary>
        private const string OAuthVersion = "1.0";

        #endregion

        private readonly ILogger _logger;


        public LTI2Api(ILogger logger)
        {
            _logger = logger;
        }

        #region Public Methods and Operators

        /// <summary>
        /// The get users for course.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="servicePattern">
        /// The service pattern.
        /// </param>
        /// <param name="lis_result_sourcedid">
        /// The LIS result sourced id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="ltiVersion">
        /// The LTI version.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> GetUsersForCourse(
            LmsCompany company, 
            string servicePattern, 
            string lis_result_sourcedid, 
            out string error, 
            string ltiVersion = null)
        {
            var result = new List<LmsUserDTO>();
            error = null;
            try
            {
                XElement response = CreateSignedRequestAndGetResponse(
                    company, 
                    servicePattern, 
                    lis_result_sourcedid, 
                    ltiVersion);
                
                bool isSuccess = response.XPathSelectElement("/statusinfo/codemajor").With(x => x.Value) == "Success";
                if (!isSuccess)
                {
                    /*
                        <codemajor>Fail</codemajor>
                        <description>Unable to validate message: 95D8A271-C3B0-44E5-99D1-051849737B12</description>
                        <severity>Error</severity>
                    */
                    error = string.Format("Error from Sakai. codemajor: {0}. description : {1}. severity : {2}.",
                        response.XPathSelectElement("/statusinfo/codemajor").With(x => x.Value),
                        response.XPathSelectElement("/statusinfo/description").With(x => x.Value),
                        response.XPathSelectElement("/statusinfo/severity").With(x => x.Value)
                        );

                    _logger.ErrorFormat("{0}. Service: {1}. LmsCompanyId: {2}.", error, servicePattern, company.Id);

                    return new List<LmsUserDTO>();
                }

                IEnumerable<XElement> members = response.XPathSelectElements("/members/member");
                foreach (XElement member in members)
                {
                    string email = member.XPathSelectElement("person_contact_email_primary").With(x => x.Value);
                    string role = member.XPathSelectElement("role").With(x => x.Value);
                    string userName = member.XPathSelectElement("person_sourcedid").With(x => x.Value);
                    string firstName = member.XPathSelectElement("person_name_given").With(x => x.Value);
                    string lastName = member.XPathSelectElement("person_name_family").With(x => x.Value);
                    string fullName = member.XPathSelectElement("person_name_full")
                        .Return(x => x.Value, firstName + " " + lastName);
                    string userId = member.XPathSelectElement("user_id").With(x => x.Value);

                    List<string> groupNames = new List<string>();
                    var groupsCollection = member.XPathSelectElement("groups");
                    if (groupsCollection != null)
                    {
                        var groups = groupsCollection.XPathSelectElements("group");
                        foreach (var group in groups)
                        {
                            string groupName = group.XPathSelectElement("title").With(x => x.Value);
                            groupNames.Add(groupName);
                        }
                    }
                    if(!groupNames.Any())
                        groupNames.Add(role);

                    foreach (var groupName in groupNames)
                    {
                        result.Add(
                        new LmsUserDTO
                        {
                            LmsRole = groupName,
                            PrimaryEmail = email,
                            LoginId = userName,
                            Id = userId,
                            Name = fullName,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LTI2Api.GetUsersForCourse", ex);
                error = ex.Message;
            }

            return result;
        }

        #endregion

        #region Methods
        
        private static XElement CreateSignedRequestAndGetResponse(
            LmsCompany company, 
            string serviceUrl, 
            string lis_result_sourcedid, 
            string ltiVersion)
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string ltiMessageType = MessageTypes.ReadMemberships, 
                   oauthNonce =
                       Convert.ToBase64String(
                           new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture))), 
                   oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture), 
                   oauthCallback = "about:blank", 
                   key = company.ConsumerKey, 
                   secret = company.SharedSecret, 
                   url = serviceUrl;

            ltiVersion = ltiVersion ?? LtiVersions.LTI1p0;

            const string BaseFormat =
                "id={0}&" 
                + "lti_message_type={1}&" 
                + "lti_version={2}&" 
                + "oauth_callback={3}&"
                + "oauth_consumer_key={4}&" 
                + "oauth_nonce={5}&" 
                + "oauth_signature_method={6}&"
                + "oauth_timestamp={7}&" 
                + "oauth_version={8}";

            string baseString = string.Format(
                BaseFormat, 
                Uri.EscapeDataString(lis_result_sourcedid), 
                ltiMessageType, 
                ltiVersion, 
                Uri.EscapeDataString(oauthCallback), 
                key, 
                oauthNonce,
                OAuthSignatureMethod, 
                oauthTimestamp, 
                OAuthVersion);

            baseString = string.Concat("POST&", Uri.EscapeDataString(url), "&", Uri.EscapeDataString(baseString));

            string compositeKey = Uri.EscapeDataString(secret) + "&";

            string oauthSignature;
            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
            }

            ServicePointManager.Expect100Continue = false;

            var request = (HttpWebRequest)WebRequest.Create(url);

            var pairs = new NameValueCollection
            {
                { "id", lis_result_sourcedid }, 
                { "lti_message_type", ltiMessageType }, 
                { "lti_version", ltiVersion }, 
                { "oauth_callback", oauthCallback }, 
                { "oauth_consumer_key", key }, 
                { "oauth_nonce", oauthNonce }, 
                { "oauth_signature", oauthSignature }, 
                { "oauth_signature_method", OAuthSignatureMethod }, 
                { "oauth_timestamp", oauthTimestamp }, 
                { "oauth_version", OAuthVersion }
            };

            var builder = new UriBuilder(url);

            foreach (string pkey in pairs.Keys)
            {
                builder.AppendQueryArgument(pkey, pairs[pkey]);
            }

            byte[] bytes = Encoding.UTF8.GetBytes(builder.Uri.Query.TrimStart("?".ToCharArray()));

            string resp;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Timeout = 5000;  // TODO: add timeout
            request.Referer = url;
            request.Host = new Uri(url).Host;
            request.ContentLength = bytes.Length;
            using (Stream requeststream = request.GetRequestStream())
            {
                requeststream.Write(bytes, 0, bytes.Length);
                requeststream.Close();
            }

            using (var webResponse = (HttpWebResponse)request.GetResponse())
            {
                // TRICK: ACLTI-478 (UNIR: Accents aren't shown correctly in Participants List)
                Encoding encoding = (company.LanguageId == LmsCompany.SpanishLanguageId)
                    ? Encoding.GetEncoding("ISO-8859-1")
                    : Encoding.UTF8;
                using (var sr = new StreamReader(webResponse.GetResponseStream(), encoding, true))
                {
                    resp = sr.ReadToEnd().Trim();
                    sr.Close();
                }

                webResponse.Close();
            }

            XElement response = XElement.Parse(resp);
            return response;
        }

        #endregion

        /// <summary>
        /// The LTI versions.
        /// </summary>
        private static class LtiVersions
        {
            #region Constants

            /// <summary>
            /// The LTI 1 P 0.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            public const string LTI1p0 = "LTI-1p0";

            #endregion
        }

        /// <summary>
        /// The message types.
        /// </summary>
        private static class MessageTypes
        {
            #region Constants

            /// <summary>
            /// The read memberships.
            /// </summary>
            public const string ReadMemberships = "basic-lis-readmembershipsforcontext";

            #endregion
        }

    }

}