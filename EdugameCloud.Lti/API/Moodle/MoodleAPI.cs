namespace EdugameCloud.Lti.API.Moodle
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Xml;

    using BbWsClient;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Constants;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    /// <summary>
    ///     The Moodle API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MoodleAPI : ILmsAPI
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
        protected readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

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

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the Moodle service short name.
        /// </summary>
        protected virtual string MoodleServiceShortName
        {
            get
            {
                return "lms";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create rest client.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleSession"/>.
        /// </returns>
        public MoodleSession BeginBatch(out string error, CompanyLms companyLms)
        {
            LmsUser lmsUser = companyLms.AdminUser;
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
            List<LmsUserDTO> enrollmentsResult = this.LoginIfNecessary(
                token, 
                c =>
                    {
                        var pairs = new NameValueCollection
                                        {
                                            { "wsfunction", "core_enrol_get_enrolled_users" }, 
                                            { "wstoken", c.Token }, 
                                            {
                                                "courseid", 
                                                courseid.ToString(CultureInfo.InvariantCulture)
                                            }
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
                            return new Tuple<List<LmsUserDTO>, string>(MoodleLmsUserParser.Parse(xmlDoc.SelectSingleNode("RESPONSE")), null);
                        }
                        catch (Exception ex)
                        {
                            return new Tuple<List<LmsUserDTO>, string>(new List<LmsUserDTO>(), string.Format("Error during parsing response: {0}; exception: {1}", resp, ex.With(x => x.Message)));
                        }
                    }, 
                company, 
                out error);

            if (enrollmentsResult == null)
            {
                error = error ?? "Moodle XML. Unable to retrive result from API";
                return result;
            }

            return enrollmentsResult;
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
        /// <param name="recursive">
        /// The recursive.
        /// </param>
        /// <returns>
        /// The <see cref="WebserviceWrapper"/>.
        /// </returns>
        public MoodleSession LoginAndCreateAClient(
            out string error, 
            bool useSsl, 
            string lmsDomain, 
            string userName, 
            string password, 
            bool recursive = false)
        {
            error = null;
            var pairs = new NameValueCollection
                            {
                                { "username", userName }, 
                                { "password", password }, 
                                { "service", this.MoodleServiceShortName }
                            };

            byte[] response;
            string url = this.GetTokenUrl(lmsDomain, useSsl);
            using (var client = new WebClient())
            {
                response = client.UploadValues(url, pairs);
            }

            string resp = Encoding.UTF8.GetString(response);
            if (!recursive && resp.Contains(@"""errorcode"":""sslonlyaccess"""))
            {
                return this.LoginAndCreateAClient(out error, true, lmsDomain, userName, password, true);
            }

            try
            {
                var token = (new JavaScriptSerializer()).Deserialize<MoodleTokenDTO>(resp);

                if (token.error != null)
                {
                    error = string.Format("Not able to login into: {0} for user: {1} due to error: {2}", lmsDomain, userName, token.error);
                    return null;
                }

                return new MoodleSession
                           {
                               Token = token.token,
                               Url = this.GetServicesUrl(lmsDomain, useSsl),
                               UseSSL = useSsl
                           };
            }
            catch (Exception ex)
            {
                error = string.Format("Not able to login into: {0} for user: {1}; response: {2}; error: {3}", lmsDomain, userName, resp, ex.With(x => x.Message));
                return null;
            }
        }

        #endregion

        #region Methods

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
        protected string FixDomain(string domain, bool useSsl)
        {
            domain = domain.ToLower();

            if (!(domain.StartsWith(HttpScheme.Http) || domain.StartsWith(HttpScheme.Https)))
            {
                if (useSsl)
                {
                    domain = HttpScheme.Https + domain;
                }
                else
                {
                    domain = HttpScheme.Http + domain;
                }
            }

            if (domain.Last() != '/')
            {
                domain = domain + '/';
            }

            if (((string)this.settings.MoodleChangeUrl).ToLower().Equals("true"))
            {
                return domain.Replace("64.27.12.61", "WIN-J0J791DL0DG").Replace("64.27.12.60", "192.168.10.60");
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
        protected string GetServicesUrl(string domain, bool useSsl)
        {
            var serviceUrl = (string)this.settings.MoodleServiceUrl;
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
        protected string GetTokenUrl(string domain, bool useSsl)
        {
            var tokenUrl = (string)this.settings.MoodleTokenUrl;
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
        protected T LoginIfNecessary<T>(
            MoodleSession session, 
            Func<MoodleSession, T> action, 
            out string error, 
            LmsUser lmsUser = null)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, lmsUser.Return(x => x.CompanyLms, null));
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
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected T LoginIfNecessary<T>(
            MoodleSession session, 
            Func<MoodleSession, T> action, 
            CompanyLms companyLms, 
            out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, companyLms);
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
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected T LoginIfNecessary<T>(
            MoodleSession session,
            Func<MoodleSession, Tuple<T, string>> action,
            CompanyLms companyLms,
            out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, companyLms);
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
        protected XmlDocument UploadValues(string url, NameValueCollection pairs)
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