﻿namespace EdugameCloud.Lti.API.BlackBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using BbWsClient;
    using BbWsClient.CourseMembership;
    using BbWsClient.User;
    using Castle.Core.Logging;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using RestSharp;

    /// <summary>
    ///     The SOAP API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SoapAPI : ILmsAPI
    {
        #region Static Fields

        /// <summary>
        /// The port regex.
        /// </summary>
        private static readonly Regex portRegex = new Regex(@":(?<port>\d+)$", RegexOptions.Compiled);

        #endregion

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
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <returns>
        /// The <see cref="RestClient"/>.
        /// </returns>
        public WebserviceWrapper BeginBatch(out string error, CompanyLms companyLms)
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
                ref client,
                c =>
                    {
                        var courseIdFixed = string.Format("_{0}_1", courseid);
                        var membershipFilter = new MembershipFilter
                                                   {
                                                       filterType = 2,
                                                       filterTypeSpecified = true,
                                                       courseIds = new[] { courseIdFixed }
                                                   };
                        
                        var membership = c.getCourseMembershipWrapper();
                        if (membership != null)
                        {
                            var enrollments = membership.loadCourseMembership(courseIdFixed, membershipFilter);
                            string errorDuringEnrollments;
                            if (this.HadError(c, out errorDuringEnrollments))
                            {
                                return new List<LmsUserDTO>();
                            }

                            if (enrollments != null)
                            {
                                var roles = membership.loadRoles(null);
                                var userFilter = new UserFilter
                                                     {
                                                         filterTypeSpecified = true,
                                                         filterType = 2,
                                                         id = enrollments.Select(x => x.userId).ToArray()
                                                     };
                                var userService = c.getUserWrapper();
                                if (userService != null)
                                {
                                    var users = c.getUserWrapper().getUser(userFilter);
                                    if (users == null)
                                    {
                                        return new List<LmsUserDTO>();
                                    }
                                    return enrollments.Select(
                                        e =>
                                            {
                                                var user = users.FirstOrDefault(u => e.userId == u.id);
                                                return new LmsUserDTO
                                                           {
                                                               id = e.userId,
                                                               login_id = user.With(x => x.name),
                                                               primary_email =
                                                                   user.With(x => x.extendedInfo)
                                                                   .With(x => x.emailAddress),
                                                               name =
                                                                   user.With(x => x.extendedInfo)
                                                                   .Return(
                                                                       x =>
                                                                       string.Format(
                                                                           "{0} {1}",
                                                                           x.givenName,
                                                                           x.familyName).Trim(),
                                                                       user.With(s => s.name)),
                                                               lms_role = this.GetRole(e.roleId, roles),
                                                           };
                                            }).ToList();
                                }
                            }
                        }

                        return new List<LmsUserDTO>();
                    },
                company,
                out error);

            if (enrollmentsResult == null)
            {
                error = error ?? "SOAP. Unable to retrive result from API";
                return result;
            }

            return enrollmentsResult;
        }

        /// <summary>
        /// The had error.
        /// </summary>
        /// <param name="ws">
        /// The WS.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HadError(WebserviceWrapper ws, out string error)
        {
            error = null;
            if (ws == null)
            {
                error = "NULL webservicewrapper";
                return true;
            }

            string lastError = ws.getLastError();
            if (lastError != null)
            {
                error = lastError;
                return true;
            }

            return false;
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
        public WebserviceWrapper LoginAndCreateAClient(
            out string error, 
            bool useSsl, 
            string lmsDomain, 
            string userName, 
            string password)
        {
            try
            {
                var client = new WebserviceWrapper(
                    this.GetHost(lmsDomain, useSsl),
                    "EGC",
                    "LTI",
                    TimeSpan.FromMinutes(30).Seconds);
                if (this.HadError(client, out error))
                {
                    return null;
                }

                client.initialize_v1();
                if (this.HadError(client, out error))
                {
                    return null;
                }

                if (client.loginUser(userName, password))
                {
                    return client;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            error = "Not able to login into: " + lmsDomain + " for user: " + userName;
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get role.
        /// </summary>
        /// <param name="roleId">
        /// The role id.
        /// </param>
        /// <param name="availableRoles">
        /// The available roles.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRole(string roleId, IEnumerable<CourseMembershipRoleVO> availableRoles)
        {
            var role =
                availableRoles.FirstOrDefault(x => x.roleIdentifier.Equals(roleId, StringComparison.OrdinalIgnoreCase))
                    .Return(x => x.courseRoleDescription, string.Empty)
                    .ToLowerInvariant()
                    .Trim(':');
            return Inflector.Capitalize(role);
        }

        /// <summary>
        /// The get host.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetHost(string lmsDomain, bool useSsl)
        {
            Match match = portRegex.Match(lmsDomain);
            bool endsWithPort = match.Success;

            lmsDomain = lmsDomain.AddHttpProtocol(useSsl);
            lmsDomain = !endsWithPort ? (useSsl ? lmsDomain + ":443" : lmsDomain + ":80") : lmsDomain;

            return lmsDomain;
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
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(ref WebserviceWrapper client, Func<WebserviceWrapper, T> action, CompanyLms companyLms, out string error)
        {
            error = null;
            client = client ?? this.BeginBatch(out error, companyLms);
            if (client != null)
            {
                return action(client);
            }

            return default(T);
        }

        #endregion
    }
}