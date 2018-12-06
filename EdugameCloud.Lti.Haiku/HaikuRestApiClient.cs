﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using EdugameCloud.Lti.API.Haiku;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using HttpScheme = EdugameCloud.Lti.Core.Constants.HttpScheme;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuRestApiClient : IHaikuRestApiClient
    {
        private readonly ILogger _logger;


        public HaikuRestApiClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(Dictionary<string, object> licenseSettings, string courseId)
        {
            var consumerKey = (string)licenseSettings[LmsLicenseSettingNames.HaikuConsumerKey];
            var consumerSecret = (string)licenseSettings[LmsLicenseSettingNames.HaikuConsumerSecret];
            var token = (string)licenseSettings[LmsLicenseSettingNames.HaikuToken];
            var tokenSecret = (string)licenseSettings[LmsLicenseSettingNames.HaikuTokenSecret];

            var scheme = (bool)licenseSettings[LmsLicenseSettingNames.UseSSL] ? HttpScheme.Https : HttpScheme.Http;
            var lmsDomain = $"{scheme}{((string)licenseSettings[LmsLicenseSettingNames.LmsDomain]).TrimEnd('/')}";

            var oAuth = new OAuthBase()
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                Token = token,
                TokenSecret = tokenSecret
            };

            string uri = $"{lmsDomain}/do/services/class/{courseId}/roster?include=user";
            string xml = await oAuth.oAuthWebRequestAsync(OAuthBase.Method.GET, uri, "");

            string error = null;
            List<LmsUserDTO> result = null;

            try
            {
                result = HaikuLmsUserParser.Parse(xml);
            }
            catch (Exception ex)
            {
                _logger.Error("Can't parse HaikuLmsUser", ex);

                error = "Can't parse Haiku Lms User.";
            }

            return (result, error);
        }

        public async Task<bool> TestOauthAsync(string lmsDomain, string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            try
            {
                var oAuth = new OAuthBase()
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    Token = token,
                    TokenSecret = tokenSecret
                };

                string uri = $"{lmsDomain}/do/services/test/oauth";
                string xml = await oAuth.oAuthWebRequestAsync(OAuthBase.Method.GET, uri, "");

                var xmlDoc = XDocument.Parse(xml);
                var response = xmlDoc.Root;
                var statusCode = response.Attribute("status").Value;
                if (!"ok".Equals(statusCode, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Invalid status {statusCode}. Xml={xml}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("TestOauth", ex);

                return false;
            }
        }

        public async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSectionsAsync(Dictionary<string, object> licenseSettings, string courseId)
        {
            var consumerKey = (string)licenseSettings[LmsLicenseSettingNames.HaikuConsumerKey];
            var consumerSecret = (string)licenseSettings[LmsLicenseSettingNames.HaikuConsumerSecret];
            var token = (string)licenseSettings[LmsLicenseSettingNames.HaikuToken];
            var tokenSecret = (string)licenseSettings[LmsLicenseSettingNames.HaikuTokenSecret];

            var scheme = (bool)licenseSettings[LmsLicenseSettingNames.UseSSL] ? HttpScheme.Https : HttpScheme.Http;
            var lmsDomain = $"{scheme}{((string)licenseSettings[LmsLicenseSettingNames.LmsDomain]).TrimEnd('/')}";

            var oAuth = new OAuthBase()
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                Token = token,
                TokenSecret = tokenSecret
            };

            string uri = $"{lmsDomain}/do/services/class/{courseId}/roster?include=user";

            string xml;

            try
            {
                xml = await oAuth.oAuthWebRequestAsync(OAuthBase.Method.GET, uri, "");
            }
            catch (Exception ex)
            {
                _logger.Error($"oAuth.oAuthWebRequestAsync uri: {uri}", ex);

                throw;
            }

            List<LmsCourseSectionDTO> result = null;

            try
            {
                result = HaikuLmsUserParser.ParseSections(xml);
            }
            catch (Exception ex)
            {
                _logger.Error($"Can't parse HaikuLmsCourseSection xml: {xml}", ex);

                throw;
            }

            return result;
        }

    }

}
