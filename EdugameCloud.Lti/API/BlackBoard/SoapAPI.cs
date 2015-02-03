﻿namespace EdugameCloud.Lti.API.BlackBoard
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text.RegularExpressions;
    using BbWsClient;
    using BbWsClient.CourseMembership;
    using BbWsClient.User;
    using Castle.Core.Logging;

    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;

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
        #region Constants

        /// <summary>
        /// The vendor EGC.
        /// </summary>
        public const string VendorEgc = "EGC";

        /// <summary>
        /// The program LTI.
        /// </summary>
        public const string ProgramLti = "LTI";

        /// <summary>
        /// The EGC LTI tool description.
        /// </summary>
        public const string EgcLtiToolDescription = "EdugameCloud Black Board Proxy Tool for LTI Integration";

        /// <summary>
        /// The tool methods.
        /// </summary>
        public readonly List<string> ToolMethods =
            new List<string>
                {
                    "Context.WS:loginTool",
                    "Context.WS:login",
                    "CourseMembership.WS:getCourseMembership",
                    "CourseMembership.WS:getCourseRoles",
                    "User.WS:getUser",
                };

        /// <summary>
        /// The ticket methods.
        /// </summary>
        public readonly List<string> TicketMethods =
            new List<string>
                {
                    "Context.WS:login",
                    "Context.WS:loginTicket",
                    "Context.WS:getMyMemberships",
                    "Context.WS:getMemberships",
                };

        #endregion

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
        /// The try register EGC tool.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <param name="registrationPassword">
        /// The registration password.
        /// </param>
        /// <param name="initialPassword">
        /// The initial password.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TryRegisterEGCTool(string lmsDomain, string registrationPassword, string initialPassword, out string error)
        {
            WebserviceWrapper client;
            if (!this.InitializeClient(this.FixHostFromString(lmsDomain), out client, out error))
            {
                return false;
            }

            var result = client.registerTool(
                EgcLtiToolDescription,
                registrationPassword,
                initialPassword,
                this.ToolMethods.ToArray(),
                this.TicketMethods.ToArray());

            if (this.HadError(client, out error))
            {
                return false;
            }

            if (result.statusSpecified && !result.status)
            {
                error = result.failureErrors.ToPlainString();
                return false;
            }

            return result.status;
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
        /// The <see cref="RestClient"/>.
        /// </returns>
        public WebserviceWrapper BeginBatch(out string error, CompanyLms companyLms)
        {
            var lmsUser = companyLms.AdminUser;
            
            if (lmsUser != null)
            {
                string defaultToolRegistrationPassword = ConfigurationManager.AppSettings["InitialBBPassword"];
                string toolPassword = string.IsNullOrWhiteSpace(companyLms.ProxyToolSharedPassword)
                                          ? defaultToolRegistrationPassword
                                          : companyLms.ProxyToolSharedPassword;
                string lmsDomain = companyLms.LmsDomain;
                bool useSsl = companyLms.UseSSL ?? false;
                return companyLms.EnableProxyToolMode == true
                           ? this.LoginToolAndCreateAClient(out error, useSsl, lmsDomain, toolPassword)
                           : this.LoginUserAndCreateAClient(out error, useSsl, lmsDomain, lmsUser.Username, lmsUser.Password);
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
        /// <param name="forceUpdate">
        /// Forces update
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> GetUsersForCourse(
            CompanyLms company, 
            int courseid, 
            out string error, 
            ref WebserviceWrapper client,
            bool forceUpdate = false)
        {
            var result = new List<LmsUserDTO>();

            var courseIdFixed = string.Format("_{0}_1", courseid);

            var enrollmentsResult = this.LoginIfNecessary(
                ref client,
                c =>
                    {
                        string errorDuringEnrollments = null;
                        var resultedList = new List<LmsUserDTO>();
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
                            if (this.HadError(c, out errorDuringEnrollments))
                            {
                                return new Tuple<List<LmsUserDTO>, string>(resultedList, errorDuringEnrollments);
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
                                    var users = userService.getUser(userFilter);
                                    if (users == null)
                                    {
                                        this.HadError(c, out errorDuringEnrollments);
                                        return new Tuple<List<LmsUserDTO>, string>(resultedList, errorDuringEnrollments);
                                    }

                                    resultedList = enrollments.Select(
                                        e =>
                                            {
                                                var user = users.FirstOrDefault(u => e.userId == u.id);
                                                var ltiIdString = user != null && user.expansionData != null
                                                                      ? user.expansionData.FirstOrDefault(
                                                                          ed =>
                                                                          ed.StartsWith("USER.UUID", StringComparison.OrdinalIgnoreCase))
                                                                      : null;
                                                if (ltiIdString != null)
                                                {
                                                    ltiIdString = ltiIdString.Substring(ltiIdString.IndexOf('=') + 1);
                                                }

                                                return new LmsUserDTO
                                                           {
                                                               id = e.userId,
                                                               login_id = user.With(x => x.name),
                                                               primary_email = user.With(x => x.extendedInfo).With(x => x.emailAddress),
                                                               name = user.With(x => x.extendedInfo).Return(x => string.Format("{0} {1}", x.givenName, x.familyName).Trim(), user.With(s => s.name)),
                                                               lms_role = this.GetRole(e.roleId, roles),
                                                               lti_id = ltiIdString
                                                           };
                                            }).ToList();
                                }
                            }
                        }

                        return new Tuple<List<LmsUserDTO>, string>(resultedList, errorDuringEnrollments);
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
        public WebserviceWrapper LoginUserAndCreateAClient(
            out string error, 
            bool useSsl, 
            string lmsDomain, 
            string userName, 
            string password)
        {
            try
            {
                var lmsDomainFixed = this.GetHost(lmsDomain, useSsl);
                WebserviceWrapper client;
                if (!this.InitializeClient(lmsDomainFixed, out client, out error))
                {
                    return null;
                }

                if (client.loginUser(userName, password))
                {
                    return client;
                }

                if (this.HadError(client, out error))
                {
                    return null;
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
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="WebserviceWrapper"/>.
        /// </returns>
        public WebserviceWrapper LoginToolAndCreateAClient(
            out string error,
            bool useSsl,
            string lmsDomain,
            string password)
        {
            try
            {
                var lmsDomainFixed = this.GetHost(lmsDomain, useSsl);
                WebserviceWrapper client;
                if (!this.InitializeClient(lmsDomainFixed, out client, out error))
                {
                    return null;
                }

                client.initialize_v1();
                if (this.HadError(client, out error))
                {
                    return null;
                }

                if (client.loginTool(password))
                {
                    return client;
                }

                if (this.HadError(client, out error))
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            error = "Not able to login into: " + lmsDomain + " for tool, password: " + password;
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
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="WebserviceWrapper"/>.
        /// </returns>
        public WebserviceWrapper LoginTicketAndCreateAClient(
            out string error,
            bool useSsl,
            string lmsDomain,
            string password)
        {
            try
            {
                var lmsDomainFixed = this.GetHost(lmsDomain, useSsl);
                WebserviceWrapper client;
                if (!this.InitializeClient(lmsDomainFixed, out client, out error))
                {
                    return null;
                }

                client.initialize_v1();
                if (this.HadError(client, out error))
                {
                    return null;
                }

                if (client.loginTicket(password))
                {
                    return client;
                }

                if (this.HadError(client, out error))
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            error = "Not able to login into: " + lmsDomain + " for tool, password: " + password;
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize client.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool InitializeClient(string lmsDomain, out WebserviceWrapper client, out string error)
        {
            client = new WebserviceWrapper(lmsDomain, VendorEgc, ProgramLti, TimeSpan.FromMinutes(30).Seconds);
            if (this.HadError(client, out error))
            {
                return false;
            }

            client.initialize_v1();
            return !this.HadError(client, out error);
        }

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
                    .Trim(':')
                    .Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .IfElse(x => x.Length <= 1, x => x.FirstOrDefault(), x => x.LastOrDefault());
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
        /// The fix host from string.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixHostFromString(string lmsDomain)
        {
            Match match = portRegex.Match(lmsDomain);
            bool endsWithPort = match.Success;
            var useSsl = lmsDomain.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase);
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
        // ReSharper disable once UnusedMember.Local
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
        private T LoginIfNecessary<T>(ref WebserviceWrapper client, Func<WebserviceWrapper, Tuple<T, string>> action, CompanyLms companyLms, out string error)
        {
            error = null;
            client = client ?? this.BeginBatch(out error, companyLms);
            if (client != null)
            {
                var result = action(client);
                error = result.Item2;
                return result.Item1;
            }

            return default(T);
        }

        #endregion
    }
}