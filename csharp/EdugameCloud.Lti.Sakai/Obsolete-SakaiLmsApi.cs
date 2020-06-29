//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Script.Serialization;
//using System.Xml;
//using EdugameCloud.Lti.DTO;
//using EdugameCloud.Lti.Extensions;
//using Esynctraining.Core.Extensions;
//using Esynctraining.Core.Logging;
//using Esynctraining.Core.Providers;
//using Esynctraining.HttpClient;
//using Esynctraining.Lti.Lms.Common.API;
//using Esynctraining.Lti.Lms.Common.Constants;
//using Esynctraining.Lti.Lms.Common.Dto;

//namespace EdugameCloud.Lti.Sakai
//{
//    public class SakaiLmsApi : ILmsAPI, ISakaiLmsApi
//    {
//        private readonly HttpClientWrapper _httpClientWrapper;
//        protected readonly ILogger _logger;
//        private readonly dynamic _settings;

//        #region Constructors and Destructors

//        /// <summary>
//        /// Initializes a new instance of the <see cref="SakaiAPI"/> class.
//        /// </summary>
//        /// <param name="settings">
//        /// The settings.
//        /// </param>
//        /// <param name="logger">
//        /// The logger.
//        /// </param>
//        public SakaiLmsApi(ApplicationSettingsProvider settings, ILogger logger)
//        {
//            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _httpClientWrapper = new HttpClientWrapper(logger);
//        }

//        #endregion

//        #region Properties

//        protected virtual string SakaiServiceShortName
//        {
//            get { return "lms"; }
//        }

//        #endregion

//        #region Public Methods and Operators

//        public async Task<(List<LmsUserDTO> Data, string Error)> GetUsersForCourseAsync(
//            Dictionary<string, object> licenseSettings,
//            string courseId)
//        {
//            try
//            {
//                if (licenseSettings == null)
//                    throw new ArgumentNullException(nameof(licenseSettings));

//                SakaiSession token = null;

//                var (data, error) = await LoginIfNecessaryAsync(
//                    token,
//                    async c =>
//                    {
//                        var pairs = new Dictionary<string, string>
//                        {
//                            { "wsfunction", "core_enrol_get_enrolled_users" },
//                            { "wstoken", c.Token },
//                            { "courseid",  courseId }
//                        };

//                        string resp = await _httpClientWrapper.PostValuesAsync(c.Url, pairs);

//                        try
//                        {
//                            var xmlDoc = new XmlDocument();
//                            xmlDoc.LoadXml(resp);
//                            return (Data: SakaiLmsUserParser.Parse(xmlDoc.SelectSingleNode("RESPONSE")), Error: (string)null);
//                        }
//                        catch (Exception ex)
//                        {
//                            _logger.ErrorFormat(ex, "[SakaiApi.GetUsersForCourse.ResponseParsing] LmsCompany key:{0}. CourseId:{1}. Response:{2}.", licenseSettings[LmsLicenseSettingNames.LicenseKey], courseId, resp);

//                            return (Data: new List<LmsUserDTO>(), Error: string.Format("Error during parsing response: {0}; exception: {1}", resp, ex.With(x => x.Message)));
//                        }
//                    },
//                    licenseSettings);

//                if (data.Data == null)
//                {
//                    error = error ?? "Sakai XML. Unable to retrive result from API";
//                    return (new List<LmsUserDTO>(), error);
//                }

//                return (Data: data.Data, Error: data.Error);
//            }
//            catch (Exception ex)
//            {
//                _logger.ErrorFormat(ex, "[SakaiApi.GetUsersForCourse] LmsCompany key:{0}. CourseId:{1}.", licenseSettings[LmsLicenseSettingNames.LicenseKey], courseId);
//                throw;
//            }
//        }

//        public async Task<(bool Data, string Error)> LoginAndCheckSessionAsync(
//            bool useSsl,
//            string lmsDomain,
//            string userName,
//            string password,
//            bool recursive = false)
//        {
//            var session = await LoginAndCreateAClientAsync(useSsl, lmsDomain, userName, password);

//            return (Data: session.session != null, Error: session.error);
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// The login and create a client.
//        /// </summary>
//        /// <param name="error">
//        /// The error.
//        /// </param>
//        /// <param name="useSsl">
//        /// The use SSL.
//        /// </param>
//        /// <param name="lmsDomain">
//        /// The LMS domain.
//        /// </param>
//        /// <param name="userName">
//        /// The user name.
//        /// </param>
//        /// <param name="password">
//        /// The password.
//        /// </param>
//        /// <param name="recursive">
//        /// The recursive.
//        /// </param>
//        /// <returns>
//        /// The <see cref="WebserviceWrapper"/>.
//        /// </returns>
//        private async Task<(SakaiSession session, string error)> LoginAndCreateAClientAsync(
//            bool useSsl,
//            string lmsDomain,
//            string userName,
//            string password,
//            bool recursive = false)
//        {
//            string resp = string.Empty;

//            try
//            {
//                var pairs = new Dictionary<string, string>
//                {
//                    { "username", userName },
//                    { "password", password },
//                    { "service", this.SakaiServiceShortName }
//                };

//                string url = this.GetTokenUrl(lmsDomain, useSsl);
//                resp = await _httpClientWrapper.PostValuesAsync(url, pairs);
//                if (!recursive && resp.Contains(@"""errorcode"":""sslonlyaccess"""))
//                {
//                    return await LoginAndCreateAClientAsync(true, lmsDomain, userName, password, true);
//                }

//                var token = new JavaScriptSerializer().Deserialize<SakaiTokenDTO>(resp);

//                if (token.error != null)
//                {
//                    var error = string.Format(
//                        "Not able to login into: {0} for user: {1} due to error: {2}",
//                        lmsDomain,
//                        userName,
//                        token.error);

//                    return (session: null, error: error);
//                }

//                return (session: new SakaiSession
//                {
//                    Token = token.token,
//                    Url = this.GetServicesUrl(lmsDomain, useSsl),
//                    UseSSL = useSsl,
//                }, error: null);
//            }
//            catch (Exception ex)
//            {
//                _logger.ErrorFormat(ex, "[SakaiApi.LoginAndCreateAClient] LmsDomain:{0}. Username:{1}. Password:{2}.", lmsDomain, userName, password);

//                var error = string.Format(
//                    "Not able to login into: {0} for user: {1};{2} error: {3}",
//                    lmsDomain,
//                    userName,
//                    string.IsNullOrWhiteSpace(resp) ? string.Empty : string.Format(" response: {0};", resp),
//                    ex.With(x => x.Message));

//                return (session: null, error: error);
//            }
//        }

//        private async Task<(SakaiSession session, string error)> BeginBatchAsync(Dictionary<string, object> licenseSettings)
//        {
//            if (licenseSettings == null)
//                throw new ArgumentNullException(nameof(licenseSettings));


//            if (licenseSettings.ContainsKey(LmsLicenseSettingNames.AdminUsername))
//            {
//                var lmsDomain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
//                bool useSsl = (bool)licenseSettings[LmsLicenseSettingNames.UseSSL];
//                return await LoginAndCreateAClientAsync(useSsl, lmsDomain, (string)licenseSettings[LmsLicenseSettingNames.AdminUsername], (string)licenseSettings[LmsLicenseSettingNames.AdminPassword]);
//            }

//            return (session: null, error: "ASP.NET Session is expired");
//        }

//        private static string FixDomain(string domain, bool useSsl)
//        {
//            domain = domain.ToLower().AddHttpProtocol(useSsl);
//            if (domain.Last() != '/')
//            {
//                domain = domain + '/';
//            }
//            return domain;
//        }

//        private string GetServicesUrl(string domain, bool useSsl)
//        {
//            var serviceUrl = (string)_settings.SakaiServiceUrl;
//            return FixDomain(domain, useSsl) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
//        }

//        private string GetTokenUrl(string domain, bool useSsl)
//        {
//            var tokenUrl = (string)_settings.SakaiTokenUrl;
//            return FixDomain(domain, useSsl) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);
//        }

//        //internal async Task<(T Data, string Error)> LoginIfNecessaryAsync<T>(
//        //    SakaiSession session,
//        //    Func<SakaiSession, Task<T>> action,
//        //    LmsUser lmsUser = null)
//        //{
//        //    string error = null;
//        //    if (session == null)
//        //    {
//        //        var beginBatchResult = await BeginBatchAsync(lmsUser.Return(x => x.LmsCompany, null));

//        //        session = session ?? beginBatchResult.session;
//        //        error = beginBatchResult.error;
//        //    }

//        //    if (session != null)
//        //    {
//        //        var data = await action(session);

//        //        return (Data: data, Error: error);
//        //    }

//        //    return (Data: default(T), Error: error);
//        //}

//        // TRICK: marked as 'internal'
//        internal async Task<(T Data, string Error)> LoginIfNecessaryAsync<T>(
//            SakaiSession session,
//            Func<SakaiSession, Task<T>> action,
//            Dictionary<string, object> licenseSettings)
//        {
//            string error = null;

//            if (session == null)
//            {
//                (session, error) = await BeginBatchAsync(licenseSettings);
//            }

//            if (session != null)
//            {
//                return (await action(session), error);
//            }

//            return (default(T), error);
//        }

//        //internal async Task<(T Data, string Error)> LoginIfNecessaryAsync<T>(
//        //    SakaiSession session,
//        //    Func<SakaiSession, Tuple<T, string>> action,
//        //    Dictionary<string, object> licenseSettings)
//        //{
//        //    string error = null;
//        //    if (session == null)
//        //    {
//        //        var beginBatchResult = await BeginBatchAsync(licenseSettings);

//        //        session = session ?? beginBatchResult.session;
//        //        error = beginBatchResult.error;
//        //    }

//        //    if (session != null && string.IsNullOrWhiteSpace(error))
//        //    {
//        //        var res = action(session);
//        //        if (res.Item2 != null)
//        //        {
//        //            error = res.Item2;
//        //        }

//        //        return (Data: res.Item1, Error: error);
//        //    }

//        //    return (Data: default(T), Error: error);
//        //}

//        protected async Task<XmlDocument> UploadValuesAsync(string url, Dictionary<string, string> pairs)
//        {
//            string resp = await _httpClientWrapper.PostValuesAsync(url, pairs);

//            try
//            {
//                var xmlDoc = new XmlDocument();
//                xmlDoc.LoadXml(resp);
//                return xmlDoc;
//            }
//            catch (XmlException)
//            {
//                _logger.Error($"Can't parse response to XML: {resp}");
//                throw;
//            }
//        }

//        #endregion

//    }

//}