using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Esynctraining.Core.Logging;
using Esynctraining.HttpClient;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Sakai
{
    public sealed class LTI2Api : ILmsAPI
    {
        private const string OAuthSignatureMethod = "HMAC-SHA1";
        private const string OAuthVersion = "1.0";

        private readonly ILogger _logger;


        public LTI2Api(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Public Methods and Operators

        public async Task<Tuple<List<LmsUserDTO>, string>> GetUsersForCourse(
            Dictionary<string, object> licenseSettings,
            string servicePattern,
            string lis_result_sourcedid,
            string ltiVersion = null)
        {
            var result = new List<LmsUserDTO>();
            try
            {
                XElement response = await CreateSignedRequestAndGetResponse(
                    licenseSettings,
                    servicePattern,
                    lis_result_sourcedid,
                    ltiVersion);

                bool isSuccess = response.XPathSelectElement("/statusinfo/codemajor")?.Value == "Success";
                if (!isSuccess)
                {
                    /*
                        <codemajor>Fail</codemajor>
                        <description>Unable to validate message: 95D8A271-C3B0-44E5-99D1-051849737B12</description>
                        <severity>Error</severity>
                    */
                    string error = string.Format("Error from Sakai. codemajor: {0}. description : {1}. severity : {2}.",
                        response.XPathSelectElement("/statusinfo/codemajor")?.Value,
                        response.XPathSelectElement("/statusinfo/description")?.Value,
                        response.XPathSelectElement("/statusinfo/severity")?.Value
                        );

                    _logger.ErrorFormat("{0}. Service: {1}. LmsCompany key: {2}.", error, servicePattern, licenseSettings[LmsLicenseSettingNames.LicenseKey]);

                    return new Tuple<List<LmsUserDTO>, string>(new List<LmsUserDTO>(), error);
                }

                IEnumerable<XElement> members = response.XPathSelectElements("/members/member");
                foreach (XElement member in members)
                {
                    string email = member.XPathSelectElement("person_contact_email_primary")?.Value;
                    string role = member.XPathSelectElement("role")?.Value;
                    string userName = member.XPathSelectElement("person_sourcedid")?.Value;
                    string firstName = member.XPathSelectElement("person_name_given")?.Value;
                    string lastName = member.XPathSelectElement("person_name_family")?.Value;
                    string fullName = member.XPathSelectElement("person_name_full") == null ? firstName + " " + lastName : member.XPathSelectElement("person_name_full").Value;
                    string userId = member.XPathSelectElement("user_id")?.Value;

                    List<string> groupNames = new List<string>();
                    // disabled "groups as custom roles" functionality as we don't have customers who use it. License option is needed
                    /*
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
                    */
                    if (!groupNames.Any())
                        groupNames.Add(role);

                    foreach (var groupName in groupNames)
                    {
                        result.Add(
                        new LmsUserDTO
                        {
                            LmsRole = groupName,
                            Email = email,
                            Login = userName,
                            Id = userId,
                            Name = fullName,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LTI2Api.GetUsersForCourse", ex);
                return new Tuple<List<LmsUserDTO>, string>(new List<LmsUserDTO>(), ex.Message);
            }
            return new Tuple<List<LmsUserDTO>, string>(result, null);
        }

        #endregion

        #region Methods

        private static async Task<XElement> CreateSignedRequestAndGetResponse(
            Dictionary<string, object> licenseSettings,
            string serviceUrl,
            string lis_result_sourcedid,
            string ltiVersion)
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string ltiMessageType = MessageTypes.ReadMemberships;
            string oauthNonce =
                       Convert.ToBase64String(
                           new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));
            string oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            string oauthCallback = "about:blank";
            string key = (string)licenseSettings[LmsLicenseSettingNames.LicenseKey];
            string secret = (string)licenseSettings[LmsLicenseSettingNames.LicenseSecret];
            string url = serviceUrl;

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

            var pairs = new Dictionary<string, string>
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

            var http = new HttpClientWrapper(TimeSpan.FromMilliseconds(5000));
            var language = (int)licenseSettings[LmsLicenseSettingNames.LanguageId];
            //TODO FIX 10 
            Encoding encoding = language == 10
                    ? Encoding.GetEncoding("ISO-8859-1")
                    : Encoding.UTF8;
            string resp = await http.PostValuesAsync(url, pairs, encoding);

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
