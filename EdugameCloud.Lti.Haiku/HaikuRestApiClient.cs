using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using EdugameCloud.Lti.API.Haiku;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuRestApiClient : IHaikuRestApiClient
    {
        private readonly ILogger _logger;


        public HaikuRestApiClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(ILmsLicense lmsCompany, int courseId)
        {
            var consumerKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuConsumerKey);
            var consumerSecret = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuConsumerSecret);
            var token = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuToken);
            var tokenSecret = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuTokenSecret);

            var scheme = lmsCompany.UseSSL.GetValueOrDefault() ? HttpScheme.Https : HttpScheme.Http;
            var lmsDomain = $"{scheme}{lmsCompany.LmsDomain.TrimEnd('/')}";

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

        public async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSectionsAsync(ILmsLicense lmsCompany, int courseId)
        {
            var consumerKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuConsumerKey);
            var consumerSecret = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuConsumerSecret);
            var token = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuToken);
            var tokenSecret = lmsCompany.GetSetting<string>(LmsCompanySettingNames.HaikuTokenSecret);

            var scheme = lmsCompany.UseSSL.GetValueOrDefault() ? HttpScheme.Https : HttpScheme.Http;
            var lmsDomain = $"{scheme}{lmsCompany.LmsDomain.TrimEnd('/')}";

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
