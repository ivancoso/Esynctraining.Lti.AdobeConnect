namespace EdugameCloud.Lti.API.Moodle
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.SessionState;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using BbWsClient;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    /// <summary>
    /// The Moodle API.
    /// </summary>
    public class MoodleAPI : ILmsAPI
    {

        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger logger;

        /// <summary>
        ///     The settings.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private readonly dynamic settings;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleAPI"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MoodleAPI(ApplicationSettingsProvider settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
        }

        /// <summary>
        /// The get users for course.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> GetUsersForCourse(
            CompanyLms company,
            int courseid,
            out string error,
            MoodleSession token = null)
        {
            var result = new List<LmsUserDTO>();
            error = null;

            var enrollmentsResult = this.LoginIfNecessary(
                token,
                c =>
                {
                    var pairs = new NameValueCollection
                            {
                                { "wsfunction", "core_enrol_get_enrolled_users" }, 
                                { "wstoken", c.Token },
                                { "courseid", courseid.ToString(CultureInfo.InvariantCulture) }
                            };

                    byte[] response;
                    using (var client = new WebClient())
                    {
                        response = client.UploadValues(c.Url, pairs);
                    }

                    var resp = Encoding.UTF8.GetString(response);

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(resp);
                    return MoodleLmsUserParser.Parse(xmlDoc.SelectSingleNode("RESPONSE"));
                },
                out error);

            if (enrollmentsResult == null)
            {
                error = error ?? "Moodle XML. Unable to retrive result from API";
                return result;
            }

            return enrollmentsResult;
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(MoodleSession session, Func<MoodleSession, T> action, out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error);
            if (session != null)
            {
                return action(session);
            }

            return default(T);
        }

        /// <summary>
        /// The create rest client.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS User.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleSession"/>.
        /// </returns>
        public MoodleSession BeginBatch(out string error, LmsUser lmsUser = null)
        {
            if (lmsUser == null)
            {
                HttpSessionState session = HttpContext.Current.With(x => x.Session);
                string companyKey = string.Format(LtiSessionKeys.CredentialsSessionKeyPattern, "moodle");
                if (session != null && session[companyKey] != null)
                {
                    var companyLms = session[companyKey] as CompanyLms;
                    lmsUser = companyLms.With(x => x.AdminUser);
                }
            }

            if (lmsUser != null)
            {
                string lmsDomain = lmsUser.CompanyLms.LmsDomain;
                bool useSsl = lmsUser.CompanyLms.UseSSL ?? false;
                return this.LoginAndCreateAClient(out error, useSsl, lmsDomain, lmsUser.Username, lmsUser.Password);
            }

            error = "ASP.NET Session is expired";
            return null;
        }

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
        /// <returns>
        /// The <see cref="WebserviceWrapper"/>.
        /// </returns>
        public MoodleSession LoginAndCreateAClient(
            out string error,
            bool useSsl,
            string lmsDomain,
            string userName,
            string password)
        {
            error = null;
            var pairs = new NameValueCollection
                            {
                                { "username", userName }, 
                                { "password", password }, 
                                { "service", "lms" }
                            };

            byte[] response;
            var url = this.GetTokenUrl(lmsDomain, useSsl);
            using (var client = new WebClient())
            {
                response = client.UploadValues(url, pairs);
            }

            var resp = Encoding.UTF8.GetString(response);

            var token = (new JavaScriptSerializer()).Deserialize<MoodleTokenDTO>(resp);

            if (token.error != null)
            {
                error = "Not able to login into: " + lmsDomain + " for user: " + userName;
                return null;
            }

            return new MoodleSession { Token = token.token, Url = this.GetServicesUrl(lmsDomain, useSsl), UseSSL = useSsl };
        }

        /// <summary>
        /// The get services url.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="useSsl">
        /// The use Ssl.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetServicesUrl(string domain, bool useSsl)
        {
            var serviceUrl = (string)this.settings.MoodleServiceUrl;
            return this.FixDomain(domain, useSsl) + (serviceUrl.First() == '/' ? serviceUrl.Substring(1) : serviceUrl);
        }

        /// <summary>
        /// The fix domain.
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <param name="useSsl">
        /// The use Ssl.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixDomain(string domain, bool useSsl)
        {
            domain = domain.ToLower();

            if (!(domain.StartsWith("http://") || domain.StartsWith("https://")))
            {
                if (useSsl)
                {
                    domain = "https://" + domain;
                }
                else
                {
                    domain = "http://" + domain;
                }
            }

            if (domain.Last() != '/')
            {
                domain = domain + '/';
            }

            if (((string)this.settings.MoodleChangeUrl).ToLower().Equals("true"))
            {
                return domain.Replace("64.27.12.61", "WIN-J0J791DL0DG").Replace("64.27.12.60", "PRO_Moodle");
            }

            return domain;
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
            var tokenUrl = (string)this.settings.MoodleTokenUrl;
            return this.FixDomain(domain, useSsl) + (tokenUrl.First() == '/' ? tokenUrl.Substring(1) : tokenUrl);
        }
    }
}
