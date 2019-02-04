using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.HttpClient;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Esynctraining.Lti.Lms.Moodle
{
    public class MoodleApi : ILmsAPI, IMoodleApi
    {
        private static readonly HttpClientWrapper _httpClientWrapper = new HttpClientWrapper(TimeSpan.FromMilliseconds(3 * 60 * 1000));
        protected readonly ILogger _logger;
        private readonly IJsonDeserializer _jsonDeserializer;


        protected virtual string MoodleServiceShortName
        {
            get { return "zoomlms"; }
        }


        public MoodleApi(IJsonDeserializer jsonDeserializer, ILogger logger)
        {
            _jsonDeserializer = jsonDeserializer ?? throw new ArgumentNullException(nameof(jsonDeserializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<(List<LmsUserDTO> users, string error)> GetUsersForCourse(Dictionary<string, object> licenseSettings, string courseId)
        {
            try
            {
                if (licenseSettings == null)
                    throw new ArgumentNullException(nameof(licenseSettings));

                MoodleSession token = null;
                string error = null;
                var result = new List<LmsUserDTO>();
                var moodleServiceToken = (string)licenseSettings[LmsLicenseSettingNames.MoodleCoreServiceToken];
                string resp = null;

                List<LmsUserDTO> enrollmentsResult = null;
                if (!string.IsNullOrEmpty(moodleServiceToken))
                {
                    enrollmentsResult = await GetUsers(moodleServiceToken, courseId, licenseSettings);
                }
                else
                {
                    var tupleEnrollmentsResult = await LoginIfNecessary(
                        token,
                        async c =>
                        {
                            try
                            {
                                var users = await GetUsers(c.Token, courseId, licenseSettings);
                                return new Tuple<List<LmsUserDTO>, string>(users, null);
                            }
                            catch (Exception ex)
                            {
                                _logger.ErrorFormat(ex,
                                    "[MoodleApi.GetUsersForCourse.ResponseParsing] LmsCompany key:{0}. CourseId:{1}. Response:{2}.",
                                    licenseSettings[LmsLicenseSettingNames.LicenseKey], courseId, resp);

                                return new Tuple<List<LmsUserDTO>, string>(new List<LmsUserDTO>(),
                                    string.Format("Error during parsing response: {0}; exception: {1}", resp,
                                        ex.Message));
                            }
                        },
                        licenseSettings);

                    //Popov
                    var enrollmentsResultTemp = await tupleEnrollmentsResult.result;
                    enrollmentsResult = enrollmentsResultTemp.Item1;
                    error = tupleEnrollmentsResult.error;
                }

                if (enrollmentsResult == null)
                {
                    error = error ?? "Moodle XML. Unable to retrive result from API";
                    return (result, error);
                }

                return (enrollmentsResult, error);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[MoodleApi.GetUsersForCourse] LmsCompany key:{0}. CourseId:{1}.", licenseSettings[LmsLicenseSettingNames.LicenseKey], courseId);
                throw;
            }
        }

        public async Task<(bool result, string error)> LoginAndCheckSession(
            bool useSsl,
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false)
        {
            string error = null;
            var tupleSession = await LoginAndCreateAClient(lmsDomain, userName, password);

            var session = tupleSession.moodleSession;
            error = tupleSession.error;

            return (session != null, error);
        }

        // TRICK: marked as 'internal'
        internal async Task<(T result, string error)> LoginIfNecessary<T>(
            MoodleSession session,
            Func<MoodleSession, T> action,
            Dictionary<string, object> licenseSettings)
        {
            string error = null;
            var tuple = await BeginBatch(licenseSettings);

            session = session ?? tuple.moodleSession;
            error = tuple.error;

            if (session != null)
            {
                var actionResult = action(session);
                return (actionResult, error);
            }

            return (default(T), error);
        }

        internal async Task<(T result, string error)> LoginIfNecessary<T>(
            MoodleSession session,
            Func<MoodleSession, Tuple<T, string>> action,
            Dictionary<string, object> licenseSettings)
        {
            string error = null;
            var tupleSession = await BeginBatch(licenseSettings);
            //session = session ?? BeginBatch(out error, lmsCompany);
            session = session ?? tupleSession.moodleSession;
            error = tupleSession.error;

            if (session != null && string.IsNullOrWhiteSpace(error))
            {
                var res = action(session);
                if (res.Item2 != null)
                {
                    error = res.Item2;
                }

                return (res.Item1, error);
            }

            return (default(T), error);
        }


        private async Task<List<LmsUserDTO>> GetUsers(string token, string courseId, Dictionary<string, object> licenseSettings)
        {
            var pairs = new Dictionary<string, string>
            {
                { "wsfunction", "core_enrol_get_enrolled_users" },
                { "wstoken", token },
                { "courseid",  courseId },
                { "options[0][name]", "userfields" },
                { "options[0][value]", "id,username,firstname,lastname,fullname,email,roles" } //decreasing response size and time, including only fields that are used by MoodleLmsUserParser
            };

            var url = GetServicesUrl(licenseSettings);
            var xmlDoc = await UploadValues(url, pairs);
            return MoodleLmsUserParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));
        }

        private async Task<(MoodleSession moodleSession, string error)> LoginAndCreateAClient(
            string lmsDomain,
            string userName,
            string password,
            bool recursive = false)
        {
            string error = null;
            string resp = string.Empty;

            try
            {
                var pairs = new Dictionary<string, string>
                {
                    { "username", userName },
                    { "password", password },
                    { "service", this.MoodleServiceShortName }
                };

                string url = GetTokenUrl(lmsDomain);
                resp = await PostValues(url, pairs);
                if (!recursive && resp.Contains(@"""errorcode"":""sslonlyaccess"""))
                {
                    return await LoginAndCreateAClient(lmsDomain, userName, password, true);
                }

                var token = _jsonDeserializer.JsonDeserialize<MoodleTokenDTO>(resp);

                if (token.error != null)
                {
                    error = string.Format(
                        "Not able to login into: {0} for user: {1} due to error: {2}",
                        lmsDomain,
                        userName,
                        token.error);

                    return (null, error);
                }

                return (new MoodleSession
                {
                    Token = token.token,
                    Url = GetServicesUrl(lmsDomain)
                }, error);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[MoodleApi.LoginAndCreateAClient] LmsDomain:{0}. Username:{1}. Password:{2}.", lmsDomain, userName, password);

                error = string.Format(
                    "Not able to login into: {0} for user: {1};{2} error: {3}",
                    lmsDomain,
                    userName,
                    string.IsNullOrWhiteSpace(resp) ? string.Empty : $" response: {resp};",
                    ex.Message);
                return (null, error);
            }
        }

        private async Task<(MoodleSession moodleSession, string error)> BeginBatch(Dictionary<string, object> licenseSettings)
        {
            string error = null;
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            if (licenseSettings.ContainsKey(LmsLicenseSettingNames.AdminUsername))
            {
                string lmsDomain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
                return await LoginAndCreateAClient(lmsDomain, (string)licenseSettings[LmsLicenseSettingNames.AdminUsername], (string)licenseSettings[LmsLicenseSettingNames.AdminPassword]);
            }

            error = "ASP.NET Session is expired";
            return (null, error);
        }

        protected async Task<XmlDocument> UploadValues(string url, Dictionary<string, string> pairs)
        {
            string resp = await PostValues(url, pairs);

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

        protected static async Task<string> PostValues(string url, Dictionary<string, string> pairs)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Non-empty value expected", nameof(url));
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            return await _httpClientWrapper.PostValuesAsync(url, pairs);
        }

        protected string GetServicesUrl(Dictionary<string, object> licenseSettings)
        {
            return GetServicesUrl((string)licenseSettings[LmsLicenseSettingNames.LmsDomain]);
        }

        protected string GetServicesUrl(string domain)
        {
            var serviceUrl = "/webservice/rest/server.php";
            return FixDomain(domain) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
        }

        protected string GetTokenUrl(string domain)
        {
            var tokenUrl = "/login/token.php";
            return FixDomain(domain) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);
        }

        private static string FixDomain(string domain)
        {
            domain = domain.ToLower().AddHttpProtocol(true);

            if (domain.Last() != '/')
            {
                domain = domain + '/';
            }

            return domain;
        }

    }
}
