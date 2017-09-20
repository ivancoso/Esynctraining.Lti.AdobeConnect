using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Sakai
{
    public class SakaiLmsApi : ILmsAPI, ISakaiLmsApi
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        #region Fields

        protected readonly ILogger _logger;

        private readonly dynamic _settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SakaiAPI"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SakaiLmsApi(ApplicationSettingsProvider settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        #endregion

        #region Properties

        protected virtual string SakaiServiceShortName
        {
            get { return "lms"; }
        }

        #endregion

        #region Public Methods and Operators

        public List<LmsUserDTO> GetUsersForCourse(
            LmsCompany company,
            int courseId,
            out string error)
        {
            try
            {
                if (company == null)
                    throw new ArgumentNullException(nameof(company));

                SakaiSession token = null;

                var result = new List<LmsUserDTO>();
                List<LmsUserDTO> enrollmentsResult = LoginIfNecessary(
                    token,
                    c =>
                    {
                        var pairs = new Dictionary<string, string>
                        {
                            { "wsfunction", "core_enrol_get_enrolled_users" },
                            { "wstoken", c.Token },
                            { "courseid",  courseId.ToString(CultureInfo.InvariantCulture) }
                        };

                        string resp = PostValues(c.Url, pairs);

                        try
                        {
                            var xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(resp);
                            return new Tuple<List<LmsUserDTO>, string>(SakaiLmsUserParser.Parse(xmlDoc.SelectSingleNode("RESPONSE")), null);
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorFormat(ex, "[SakaiApi.GetUsersForCourse.ResponseParsing] LmsCompanyId:{0}. CourseId:{1}. Response:{2}.", company.Id, courseId, resp);

                            return new Tuple<List<LmsUserDTO>, string>(new List<LmsUserDTO>(), string.Format("Error during parsing response: {0}; exception: {1}", resp, ex.With(x => x.Message)));
                        }
                    },
                    company,
                    out error);

                if (enrollmentsResult == null)
                {
                    error = error ?? "Sakai XML. Unable to retrive result from API";
                    return result;
                }

                return enrollmentsResult;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[SakaiApi.GetUsersForCourse] LmsCompanyId:{0}. CourseId:{1}.", company.Id, courseId);
                throw;
            }
        }

        public bool LoginAndCheckSession(
            out string error,
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false)
        {
            var session = LoginAndCreateAClient(out error, useSsl, lmsDomain, userName, password);
            return session != null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The login and create a client.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="recursive">
        /// The recursive.
        /// </param>
        /// <returns>
        /// The <see cref="WebserviceWrapper"/>.
        /// </returns>
        private SakaiSession LoginAndCreateAClient(
            out string error,
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false)
        {
            error = null;
            string resp = string.Empty;

            try
            {
                var pairs = new Dictionary<string, string>
                {
                    { "username", userName },
                    { "password", password },
                    { "service", this.SakaiServiceShortName }
                };

                string url = this.GetTokenUrl(lmsDomain, useSsl);
                resp = PostValues(url, pairs);
                if (!recursive && resp.Contains(@"""errorcode"":""sslonlyaccess"""))
                {
                    return LoginAndCreateAClient(out error, true, lmsDomain, userName, password, true);
                }

                var token = new JavaScriptSerializer().Deserialize<SakaiTokenDTO>(resp);

                if (token.error != null)
                {
                    error = string.Format(
                        "Not able to login into: {0} for user: {1} due to error: {2}",
                        lmsDomain,
                        userName,
                        token.error);
                    return null;
                }

                return new SakaiSession
                {
                    Token = token.token,
                    Url = this.GetServicesUrl(lmsDomain, useSsl),
                    UseSSL = useSsl,
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[SakaiApi.LoginAndCreateAClient] LmsDomain:{0}. Username:{1}. Password:{2}.", lmsDomain, userName, password);

                error = string.Format(
                    "Not able to login into: {0} for user: {1};{2} error: {3}",
                    lmsDomain,
                    userName,
                    string.IsNullOrWhiteSpace(resp) ? string.Empty : string.Format(" response: {0};", resp),
                    ex.With(x => x.Message));
                return null;
            }
        }

        private SakaiSession BeginBatch(out string error, ILmsLicense lmsCompany)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            LmsUser lmsUser = lmsCompany.AdminUser;
            if (lmsUser != null)
            {
                string lmsDomain = lmsUser.LmsCompany.LmsDomain;
                bool useSsl = lmsUser.LmsCompany.UseSSL ?? false;
                return LoginAndCreateAClient(out error, useSsl, lmsDomain, lmsUser.Username, lmsUser.Password);
            }

            error = "ASP.NET Session is expired";
            return null;
        }

        private static string FixDomain(string domain, bool useSsl)
        {
            domain = domain.ToLower().AddHttpProtocol(useSsl);
            if (domain.Last() != '/')
            {
                domain = domain + '/';
            }
            return domain;
        }

        private string GetServicesUrl(string domain, bool useSsl)
        {
            var serviceUrl = (string)_settings.SakaiServiceUrl;
            return FixDomain(domain, useSsl) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
        }

        private string GetTokenUrl(string domain, bool useSsl)
        {
            var tokenUrl = (string)_settings.SakaiTokenUrl;
            return FixDomain(domain, useSsl) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);
        }

        internal T LoginIfNecessary<T>(
            SakaiSession session,
            Func<SakaiSession, T> action,
            out string error,
            LmsUser lmsUser = null)
        {
            error = null;
            session = session ?? BeginBatch(out error, lmsUser.Return(x => x.LmsCompany, null));
            if (session != null)
            {
                return action(session);
            }

            return default(T);
        }

        // TRICK: marked as 'internal'
        internal T LoginIfNecessary<T>(
            SakaiSession session,
            Func<SakaiSession, T> action,
            ILmsLicense lmsCompany,
            out string error)
        {
            error = null;
            session = session ?? BeginBatch(out error, lmsCompany);
            if (session != null)
            {
                return action(session);
            }

            return default(T);
        }

        internal T LoginIfNecessary<T>(
            SakaiSession session,
            Func<SakaiSession, Tuple<T, string>> action,
            ILmsLicense lmsCompany,
            out string error)
        {
            error = null;
            session = session ?? BeginBatch(out error, lmsCompany);
            if (session != null && string.IsNullOrWhiteSpace(error))
            {
                var res = action(session);
                if (res.Item2 != null)
                {
                    error = res.Item2;
                }

                return res.Item1;
            }

            return default(T);
        }

        protected XmlDocument UploadValues(string url, Dictionary<string, string> pairs)
        {
            string resp = PostValues(url, pairs);

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);
                return xmlDoc;
            }
            catch (XmlException)
            {
                _logger.Error($"Can't parse response to XML: {resp}");
                throw;
            }
        }

        protected static string PostValues(string url, Dictionary<string, string> pairs)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            return _httpClient.PostAsync(url, new FormUrlEncodedContent(pairs)).Result.Content.ReadAsStringAsync().Result;
        }

        #endregion

    }

}