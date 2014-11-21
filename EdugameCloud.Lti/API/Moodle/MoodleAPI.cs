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

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Constants;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.EntityParsing;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The Moodle API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
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

        #region Properties

        /// <summary>
        /// Gets the quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
            }
        }

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
        /// The get items info for user.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The lms user parameters.
        /// </param>
        /// <param name="isSurvey">
        /// The is survey.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<LmsQuizInfoDTO> GetItemsInfoForUser(LmsUserParameters lmsUserParameters, bool isSurvey, out string error)
        {
            var quizResult = this.LoginIfNecessary(
                null,
                c =>
                {
                    var pairs = new NameValueCollection
                                        {
                                            {
                                                "wsfunction",
                                                isSurvey ? "local_edugamecloud_get_total_survey_list" : "local_edugamecloud_get_total_quiz_list"
                                            },
                                            { "wstoken", c.Token },
                                            {
                                                "course",
                                                lmsUserParameters.Course.ToString(
                                                    CultureInfo.InvariantCulture)
                                            }
                                        };

                    var xmlDoc = this.UploadValues(c.Url, pairs);

                    return MoodleQuizInfoParser.Parse(xmlDoc, isSurvey);
                },
                out error,
                lmsUserParameters.LmsUser);

            if (quizResult == null)
            {
                error = error ?? "Moodle XML. Unable to retrive result from API";
                return new List<LmsQuizInfoDTO>();
            }

            error = string.Empty;
            return quizResult;
        }

        /// <summary>
        /// The get quzzes for user.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The lms User Parameters.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <param name="quizIds">
        /// The quiz Ids.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public IEnumerable<LmsQuizDTO> GetItemsForUser(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds, out string error)
        {
            var result = new List<LmsQuizDTO>();
            
            foreach (var quizId in quizIds)
            {
                int id = quizId;
                var quizResult = this.LoginIfNecessary(
                    null,
                    c =>
                        {
                            var pairs = new NameValueCollection
                                            {
                                                {
                                                    "wsfunction",
                                                    isSurvey ? "local_edugamecloud_get_survey_by_id" : "local_edugamecloud_get_quiz_by_id"
                                                },
                                                { "wstoken", c.Token },
                                                { 
                                                    isSurvey ? "surveyId" : "quizId", 
                                                    id.ToString(CultureInfo.InvariantCulture) 
                                                }
                                            };

                            var xmlDoc = this.UploadValues(c.Url, pairs);


                            string errorMessage = string.Empty, err = string.Empty;
                            return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
                        },
                    out error,
                    lmsUserParameters.LmsUser);
                if (quizResult == null)
                {
                    error = error ?? "Moodle XML. Unable to retrive result from API";
                    return result;
                }

                result.Add(quizResult);
            }
            
            error = string.Empty;
            return result;
        }

        /// <summary>
        /// The send answers.
        /// </summary>
        /// <param name="lmsUserParameters">
        /// The lms user parameters.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        public void SendAnswers(LmsUserParameters lmsUserParameters, string json)
        {
            string error;

            var quizResult = this.LoginIfNecessary(
                null,
                c =>
                    {
                        var pairs = new NameValueCollection
                                        {
                                            {
                                                "wsfunction",
                                                "local_edugamecloud_save_external_quiz_report"
                                            },
                                            { "wstoken", c.Token },
                                            { "reportObject", json }
                                        };

                        var xmlDoc = this.UploadValues(c.Url, pairs);


                        string errorMessage = string.Empty, err = string.Empty;
                        return MoodleQuizParser.Parse(xmlDoc, ref errorMessage, ref err);
                    },
                out error,
                lmsUserParameters.LmsUser);
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
        private XmlDocument UploadValues(string url, NameValueCollection pairs)
        {
            byte[] response;
            using (var client = new WebClient())
            {
                response = client.UploadValues(url, pairs);
            }

            var resp = Encoding.UTF8.GetString(response);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);
            return xmlDoc;
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
        /// The lms User.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(MoodleSession session, Func<MoodleSession, T> action, out string error, LmsUser lmsUser = null)
        {
            error = null;
            session = session ?? this.BeginBatch(out error, lmsUser.CompanyLms);
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
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <returns>
        /// The <see cref="MoodleSession"/>.
        /// </returns>
        public MoodleSession BeginBatch(out string error, CompanyLms companyLms)
        {
            var lmsUser = companyLms.AdminUser;
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
                                { "service", "edugamecloud" }
                            };

            byte[] response;
            var url = this.GetTokenUrl(lmsDomain, useSsl);
            using (var client = new WebClient())
            {
                response = client.UploadValues(url, pairs);
            }

            var resp = Encoding.UTF8.GetString(response);
            if (!recursive && resp.Contains(@"""errorcode"":""sslonlyaccess"""))
            {
                return this.LoginAndCreateAClient(out error, true, lmsDomain, userName, password, true);
            }

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
        /// The use SSL.
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
        private T LoginIfNecessary<T>(MoodleSession session, Func<MoodleSession, T> action, CompanyLms companyLms, out string error)
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
