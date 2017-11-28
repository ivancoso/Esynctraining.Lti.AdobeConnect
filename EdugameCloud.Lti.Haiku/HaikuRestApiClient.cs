using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
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

        public List<LmsUserDTO> GetUsersForCourse(ILmsLicense lmsCompany, int courseId, out string error)
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
            var method = OAuthBase.Method.GET;
            string xml = oAuth.oAuthWebRequest(method, uri, "");

            error = null;
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

            return result;
        }

        public bool TestOauth(string lmsDomain, string consumerKey, string consumerSecret, string token, string tokenSecret)
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
                var method = OAuthBase.Method.GET;
                string xml = oAuth.oAuthWebRequest(method, uri, "");

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

        public List<LmsCourseSectionDTO> GetCourseSections(ILmsLicense lmsCompany, int courseId, out string error)
        {
            try
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

                var method = OAuthBase.Method.GET;
                string xml = oAuth.oAuthWebRequest(method, uri, "");

                error = null;
                List<LmsCourseSectionDTO> result = null;

                try
                {
                    result = HaikuLmsUserParser.ParseSections(xml);
                }
                catch (Exception ex)
                {
                    _logger.Error("Can't parse HaikuLmsCourseSection", ex);

                    error = "Can't parse Haiku Lms course section.";
                }

                return result;
            }
            catch (Exception ex)
            {
                //_logger.ErrorFormat(ex, "[HaikuRestApiClient.GetCourseSections] API:{0}. UserToken:{1}. CourseId:{2}.", domain, userToken, courseId);
                throw;
            }
        }

    }
}
