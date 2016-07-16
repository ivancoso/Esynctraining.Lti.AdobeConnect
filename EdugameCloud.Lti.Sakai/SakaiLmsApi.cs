using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
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
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once InconsistentNaming
        protected readonly ILogger logger;

        /// <summary>
        ///     The settings.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once InconsistentNaming
        private readonly dynamic settings;

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
            this.settings = settings;
            this.logger = logger;
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
                    throw new ArgumentNullException("company");

                SakaiSession token = null;

                var result = new List<LmsUserDTO>();
                List<LmsUserDTO> enrollmentsResult = this.LoginIfNecessary(
                    token,
                    c =>
                    {
                        var pairs = new NameValueCollection
                        {
                            { "wsfunction", "core_enrol_get_enrolled_users" },
                            { "wstoken", c.Token },
                            { "courseid",  courseId.ToString(CultureInfo.InvariantCulture) }
                        };

                        byte[] response;
                        using (var client = new WebClient())
                        {
                            response = client.UploadValues(c.Url, pairs);
                        }

                        string resp = Encoding.UTF8.GetString(response);

                        try
                        {
                            var xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(resp);
                            return new Tuple<List<LmsUserDTO>, string>(SakaiLmsUserParser.Parse(xmlDoc.SelectSingleNode("RESPONSE")), null);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat(ex, "[SakaiApi.GetUsersForCourse.ResponseParsing] LmsCompanyId:{0}. CourseId:{1}. Response:{2}.", company.Id, courseId, resp);

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
                logger.ErrorFormat(ex, "[SakaiApi.GetUsersForCourse] LmsCompanyId:{0}. CourseId:{1}.", company.Id, courseId);
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
                var pairs = new NameValueCollection
                {
                    { "username", userName },
                    { "password", password },
                    { "service", this.SakaiServiceShortName }
                };

                byte[] response;
                string url = this.GetTokenUrl(lmsDomain, useSsl);

                using (var client = new WebClient())
                {
                    response = client.UploadValues(url, pairs);
                }

                resp = Encoding.UTF8.GetString(response);
                if (!recursive && resp.Contains(@"""errorcode"":""sslonlyaccess"""))
                {
                    return this.LoginAndCreateAClient(out error, true, lmsDomain, userName, password, true);
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
                logger.ErrorFormat(ex, "[SakaiApi.LoginAndCreateAClient] LmsDomain:{0}. Username:{1}. Password:{2}.", lmsDomain, userName, password);

                error = string.Format(
                    "Not able to login into: {0} for user: {1};{2} error: {3}",
                    lmsDomain,
                    userName,
                    string.IsNullOrWhiteSpace(resp) ? string.Empty : string.Format(" response: {0};", resp),
                    ex.With(x => x.Message));
                return null;
            }
        }

        /// <summary>
        /// The create rest client.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="lmsCompany">
        /// The company LMS.
        /// </param>
        /// <returns>
        /// The <see cref="SakaiSession"/>.
        /// </returns>
        private SakaiSession BeginBatch(out string error, LmsCompany lmsCompany)
        {
            if (lmsCompany == null)
            {
                error = "No company lms settings";
                return null;
            }
            LmsUser lmsUser = lmsCompany.AdminUser;
            if (lmsUser != null)
            {
                string lmsDomain = lmsUser.LmsCompany.LmsDomain;
                bool useSsl = lmsUser.LmsCompany.UseSSL ?? false;
                return this.LoginAndCreateAClient(out error, useSsl, lmsDomain, lmsUser.Username, lmsUser.Password);
            }

            error = "ASP.NET Session is expired";
            return null;
        }

        /// <summary>
        /// The fix domain.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixDomain(string domain, bool useSsl)
        {
            domain = domain.ToLower().AddHttpProtocol(useSsl);

            if (domain.Last() != '/')
            {
                domain = domain + '/';
            }

            if (((string)this.settings.SakaiChangeUrl).Equals("TRUE", StringComparison.OrdinalIgnoreCase))
            {
                return domain.Replace("64.27.12.61", "WIN-J0J791DL0DG").Replace("64.27.12.60", "192.168.10.60").Replace("Sakai.esynctraining.com", "192.168.10.60");
            }

            return domain;
        }

        /// <summary>
        /// The get services url.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetServicesUrl(string domain, bool useSsl)
        {
            var serviceUrl = (string)this.settings.SakaiServiceUrl;
            return this.FixDomain(domain, useSsl) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
        }

        /// <summary>
        /// The get token url.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTokenUrl(string domain, bool useSsl)
        {
            var tokenUrl = (string)this.settings.SakaiTokenUrl;
            return this.FixDomain(domain, useSsl) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS User.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal T LoginIfNecessary<T>(
            SakaiSession session,
            Func<SakaiSession, T> action,
            out string error,
            LmsUser lmsUser = null)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, lmsUser.Return(x => x.LmsCompany, null));
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
            LmsCompany lmsCompany,
            out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, lmsCompany);
            if (session != null)
            {
                return action(session);
            }

            return default(T);
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="lmsCompany">
        /// The company LMS.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal T LoginIfNecessary<T>(
            SakaiSession session,
            Func<SakaiSession, Tuple<T, string>> action,
            LmsCompany lmsCompany,
            out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, lmsCompany);
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

        /// <summary>
        /// The upload values.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="pairs">
        /// The pairs.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        protected static XmlDocument UploadValues(string url, NameValueCollection pairs)
        {
            byte[] response;
            using (var client = new WebClient())
            {
                response = client.UploadValues(url, pairs);
            }

            string resp = Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);
            return xmlDoc;
        }

        #endregion

    }
}