namespace EdugameCloud.Lti.API.BlackBoard
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Web;
    using System.Web.SessionState;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using BbWsClient;
    using BbWsClient.CourseMembership;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using RestSharp;

    /// <summary>
    ///     The SOAP API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SoapAPI
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapAPI"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SoapAPI(ApplicationSettingsProvider settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
        }

        #endregion

        #region Public Methods and Operators

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
        /// The <see cref="RestClient"/>.
        /// </returns>
        public WebserviceWrapper BeginBatch(out string error, LmsUser lmsUser = null)
        {
            if (lmsUser == null)
            {
                HttpSessionState session = HttpContext.Current.With(x => x.Session);
                string companyKey = string.Format(LtiSessionKeys.CredentialsSessionKeyPattern, "blackboard");
                if (session != null && session[companyKey] != null)
                {
                    var companyLms = session[companyKey] as CompanyLms;
                    lmsUser = companyLms.With(x => x.AdminUser);
                }
            }

            if (lmsUser != null)
            {
                string lmsDomain = lmsUser.CompanyLms.LmsDomain;
                return this.LoginAndCreateAClient(out error, lmsDomain, lmsUser.Username, lmsUser.Password);
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
        public WebserviceWrapper LoginAndCreateAClient(out string error, string lmsDomain, string userName, string password)
        {
            error = null;
            var client = new WebserviceWrapper(
                "http://" + lmsDomain,
                "EduGameCloud",
                string.Empty,
                TimeSpan.FromMinutes(15).Ticks);
            if (client.loginUser(userName, password))
            {
                return client;
            }

            error = "Not able to login into: " + lmsDomain + " for user: " + userName;
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
        /// <param name="client">
        /// The client.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> GetUsersForCourse(
            CompanyLms company, 
            int courseid, 
            out string error, 
            WebserviceWrapper client = null)
        {
            var result = new List<LmsUserDTO>();
            var enrollmentsResult = this.LoginIfNecessary(
                client, 
                c =>
                c.getCourseMembershipWrapper().loadGroupMembership("_" + courseid + "_1", new MembershipFilter())
                , 
                out error);
            if (enrollmentsResult == null)
            {
                error = error ?? "SOAP. Unable to retrive result from API";
                return result;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <param name="session">
        /// The client.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private void LoginIfNecessary(WebserviceWrapper session, Action<WebserviceWrapper> action)
        {
            string error = null;
            session = session ?? this.BeginBatch(out error);
            action(session);
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="session">
        /// The client.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(WebserviceWrapper session, Func<WebserviceWrapper, T> action)
        {
            string error = null;
            session = session ?? this.BeginBatch(out error);
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
        private T LoginIfNecessary<T>(WebserviceWrapper client, Func<WebserviceWrapper, T> action, out string error)
        {
            error = null;
            client = client ?? this.BeginBatch(out error);
            if (client != null)
            {
                return action(client);
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
        private T LoginIfNecessary<T>(WebserviceWrapper session, Func<WebserviceWrapper, Tuple<T, string>> action, out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error);
            if (session != null)
            {
                Tuple<T, string> resTuple = action(session);
                error = resTuple.Item2;
                return resTuple.Item1;
            }

            error = error ?? "SOAP. Client is null";
            return default(T);
        }

        #endregion
    }
}