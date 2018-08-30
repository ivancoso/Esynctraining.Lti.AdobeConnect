using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Esynctraining.BlackBoardClient;
using Esynctraining.BlackBoardClient.Announcements;
using Esynctraining.BlackBoardClient.CourseMembership;
using Esynctraining.BlackBoardClient.User;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.BlackBoard
{
    //public class SoapBlackBoardApi : ILmsAPI, IBlackBoardApi
    //{
    //    #region Constants

    //    /// <summary>
    //    /// The vendor EGC.
    //    /// </summary>
    //    public const string VendorEgc = "EGC";

    //    /// <summary>
    //    /// The program LTI.
    //    /// </summary>
    //    public const string ProgramLti = "LTI";

    //    /// <summary>
    //    /// The EGC LTI tool description.
    //    /// </summary>
    //    public const string EgcLtiToolDescription = "EdugameCloud Black Board Proxy Tool for LTI Integration";

    //    /// <summary>
    //    /// The tool methods.
    //    /// </summary>
    //    public readonly List<string> ToolMethods =
    //        new List<string>
    //            {
    //                "Context.WS:loginTool",
    //                "Context.WS:login",
    //                "Context.WS:emulateUser",
    //                "CourseMembership.WS:getCourseMembership",
    //                "CourseMembership.WS:getCourseRoles",
    //                "User.WS:getUser",
    //                "Announcement.WS:createCourseAnnouncements",
    //                "Content.WS:getTOCsByCourseId",
    //                "Content.WS:getFilteredContent",
    //                "Content.WS:loadFilteredContent"
    //            };

    //    /// <summary>
    //    /// The ticket methods.
    //    /// </summary>
    //    public readonly List<string> TicketMethods =
    //        new List<string>
    //            {
    //                "Context.WS:login",
    //                "Context.WS:loginTicket",
    //                "Context.WS:getMyMemberships",
    //                "Context.WS:getMemberships",
    //            };

    //    #endregion

    //    #region Static Fields

    //    /// <summary>
    //    /// The port regex.
    //    /// </summary>
    //    private static readonly Regex portRegex = new Regex(@":(?<port>\d+)$", RegexOptions.Compiled);

    //    #endregion

    //    #region Fields

    //    private readonly ILogger _logger;

    //    #endregion

    //    #region Constructors and Destructors

    //    public SoapBlackBoardApi(ILogger logger)
    //    {
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }

    //    #endregion

    //    #region Public Methods and Operators

    //    /// <summary>
    //    /// The try register EGC tool.
    //    /// </summary>
    //    /// <param name="lmsDomain">
    //    /// The LMS domain.
    //    /// </param>
    //    /// <param name="registrationPassword">
    //    /// The registration password.
    //    /// </param>
    //    /// <param name="initialPassword">
    //    /// The initial password.
    //    /// </param>
    //    /// <param name="error">
    //    /// The error.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="bool"/>.
    //    /// </returns>
    //    public bool TryRegisterEGCTool(string lmsDomain, string registrationPassword, string initialPassword, out string error)
    //    {
    //        WebserviceWrapper client;
    //        if (!InitializeClient(FixHostFromString(lmsDomain), out client, out error))
    //        {
    //            return false;
    //        }

    //        var result = client.registerTool(
    //            EgcLtiToolDescription,
    //            registrationPassword,
    //            initialPassword,
    //            ToolMethods.ToArray(),
    //            TicketMethods.ToArray());

    //        if (HadError(client, out error))
    //        {
    //            return false;
    //        }

    //        if (result.statusSpecified && !result.status)
    //        {
    //            error = ToPlainString(result.failureErrors);
    //            return false;
    //        }

    //        return result.status;
    //    }

    //    private string ToPlainString(IEnumerable<string> value)
    //    {
    //        StringBuilder stringBuilder = new StringBuilder();
    //        foreach (string str in value)
    //            stringBuilder.AppendFormat(" {0},", (object)str);
    //        return stringBuilder.ToString().TrimEnd(",".ToCharArray());
    //    }

    //    public List<LmsUserDTO> GetUsersForCourse(
    //        Dictionary<string, object> licenseSettings,
    //        string courseId,
    //        string[] userIds,
    //        out string error,
    //        ref WebserviceWrapper client)
    //    {
    //        var courseIdFixed = string.Format("_{0}_1", courseId);

    //        var enrollmentsResult = LoginIfNecessary(
    //            ref client,
    //            c =>
    //            {
    //                string errorDuringEnrollments = null;
    //                var resultedList = new List<LmsUserDTO>();

    //                // NOTE: check http://library.blackboard.com/ref/fd8c40a0-f670-4c48-9e02-c5d84e61eda7/blackboard/ws/coursemembership/CourseMembershipWS.html
    //                // for filterType
    //                //var membershipFilter = new MembershipFilter
    //                //{
    //                //    filterType = (userIds == null) ? 2 : 6,
    //                //    filterTypeSpecified = true,
    //                //    courseIds = new[] { courseIdFixed },
    //                //    userIds = userIds,
    //                //};
    //                var membershipFilter = new MembershipFilter
    //                {
    //                    filterType = 2,
    //                    filterTypeSpecified = true,
    //                    courseIds = new[] { courseIdFixed },
    //                };

    //                CourseMembershipWrapper membership = c.getCourseMembershipWrapper();

    //                if (membership != null)
    //                {
    //                    CourseMembershipVO[] enrollments = membership.loadCourseMembership(courseIdFixed, membershipFilter);
    //                    if (HadError(c, out errorDuringEnrollments))
    //                    {
    //                        return new Tuple<List<LmsUserDTO>, string>(resultedList, errorDuringEnrollments);
    //                    }

    //                    if (enrollments != null)
    //                    {
    //                        var activeEnrollments = enrollments.Where(x => x.available.HasValue && x.available.Value);

    //                        CourseMembershipRoleVO[] roles = membership.loadRoles(null);
    //                        var userFilter = new UserFilter
    //                        {
    //                            filterTypeSpecified = true,
    //                            filterType = 2,
    //                            id = activeEnrollments.Select(x => x.userId).ToArray(),
    //                        };
    //                        UserWrapper userService = c.getUserWrapper();
    //                        if (userService != null)
    //                        {
    //                            UserVO[] users = userService.getUser(userFilter);
    //                            if (users == null)
    //                            {
    //                                HadError(c, out errorDuringEnrollments);
    //                                return new Tuple<List<LmsUserDTO>, string>(resultedList, errorDuringEnrollments);
    //                            }

    //                            resultedList = activeEnrollments.Select(
    //                                e =>
    //                                {
    //                                    var user = users.FirstOrDefault(u => e.userId == u.id);
    //                                    var ltiIdString = user != null && user.expansionData != null
    //                                        ? user.expansionData.FirstOrDefault(ed => ed.StartsWith("USER.UUID", StringComparison.OrdinalIgnoreCase))
    //                                        : null;
    //                                    if (ltiIdString != null)
    //                                    {
    //                                        ltiIdString = ltiIdString.Substring(ltiIdString.IndexOf('=') + 1);
    //                                    }

    //                                    return new LmsUserDTO
    //                                    {
    //                                        Id = e.userId,
    //                                        Login = user?.name,
    //                                        PrimaryEmail = user?.extendedInfo?.emailAddress,
    //                                        Name = user?.extendedInfo == null ? user?.name : $"{user.extendedInfo.givenName} {user.extendedInfo.familyName}".Trim(),
    //                                        LmsRole = GetRole(e.roleId, roles),
    //                                        LtiId = ltiIdString,
    //                                    };
    //                                }).ToList();
    //                        }
    //                    }
    //                }

    //                return new Tuple<List<LmsUserDTO>, string>(resultedList, errorDuringEnrollments);
    //            },
    //            licenseSettings,
    //            out error);

    //        if (enrollmentsResult == null)
    //        {
    //            error = error ?? "SOAP. Unable to retrive result from API";
    //            return new List<LmsUserDTO>();
    //        }
    //        return enrollmentsResult;
    //    }

    //    /// <summary>
    //    /// The login and create a client.
    //    /// </summary>
    //    /// <param name="error">
    //    /// The error.
    //    /// </param>
    //    /// <param name="useSsl">
    //    /// The use SSL.
    //    /// </param>
    //    /// <param name="lmsDomain">
    //    /// The LMS domain.
    //    /// </param>
    //    /// <param name="userName">
    //    /// The user name.
    //    /// </param>
    //    /// <param name="password">
    //    /// The password.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="WebserviceWrapper"/>.
    //    /// </returns>
    //    public WebserviceWrapper LoginUserAndCreateAClient(
    //        out string error,
    //        bool useSsl,
    //        string lmsDomain,
    //        string userName,
    //        string password)
    //    {
    //        try
    //        {
    //            var lmsDomainFixed = GetHost(lmsDomain, useSsl);
    //            WebserviceWrapper client;
    //            if (!InitializeClient(lmsDomainFixed, out client, out error))
    //            {
    //                return null;
    //            }

    //            if (client.loginUser(userName, password))
    //            {
    //                return client;
    //            }

    //            if (HadError(client, out error))
    //            {
    //                return null;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            error = ex.Message;
    //            return null;
    //        }

    //        error = "Not able to login into: " + lmsDomain + " for user: " + userName;
    //        return null;
    //    }

    //    /// <summary>
    //    /// The login and create a client.
    //    /// </summary>
    //    /// <param name="error">
    //    /// The error.
    //    /// </param>
    //    /// <param name="useSsl">
    //    /// The use SSL.
    //    /// </param>
    //    /// <param name="lmsDomain">
    //    /// The LMS domain.
    //    /// </param>
    //    /// <param name="password">
    //    /// The password.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="WebserviceWrapper"/>.
    //    /// </returns>
    //    public WebserviceWrapper LoginToolAndCreateAClient(
    //        out string error,
    //        bool useSsl,
    //        string lmsDomain,
    //        string password)
    //    {
    //        try
    //        {
    //            var lmsDomainFixed = GetHost(lmsDomain, useSsl);
    //            WebserviceWrapper client;
    //            if (!InitializeClient(lmsDomainFixed, out client, out error))
    //            {
    //                return null;
    //            }

    //            if (client.loginTool(password))
    //            {
    //                return client;
    //            }

    //            if (HadError(client, out error))
    //            {
    //                return null;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            error = ex.Message;
    //            return null;
    //        }

    //        error = "Not able to login into: " + lmsDomain + " for tool, password: " + password;
    //        return null;
    //    }

    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    /// The create rest client.
    //    /// </summary>
    //    /// <param name="error">
    //    /// The error.
    //    /// </param>
    //    /// <param name="lmsCompany">
    //    /// The company LMS.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="RestClient"/>.
    //    /// </returns>
    //    private WebserviceWrapper BeginBatch(out string error, Dictionary<string, object> licenseSettings)
    //    {
    //        var lmsUser = licenseSettings[LmsLicenseSettingNames.BlackBoardAdminUser];

    //        if (lmsUser != null || (bool)licenseSettings[LmsLicenseSettingNames.BlackBoardEnableProxyToolMode] == true)
    //        {
    //            string defaultToolRegistrationPassword = licenseSettings[LmsLicenseSettingNames.BlackBoardInitialBBPassword].ToString();
    //            string toolPassword = string.IsNullOrWhiteSpace(licenseSettings[LmsLicenseSettingNames.BlackBoardProxyToolSharedPassword].ToString())
    //                                      ? defaultToolRegistrationPassword
    //                                      : licenseSettings[LmsLicenseSettingNames.BlackBoardProxyToolSharedPassword].ToString();
    //            string lmsDomain = licenseSettings[LmsLicenseSettingNames.LmsDomain].ToString();
    //            bool useSsl = (bool)licenseSettings[LmsLicenseSettingNames.BlackBoardUseSSL];
    //            return (bool)licenseSettings[LmsLicenseSettingNames.BlackBoardEnableProxyToolMode] == true
    //                       ? LoginToolAndCreateAClient(out error, useSsl, lmsDomain, toolPassword)
    //                       : LoginUserAndCreateAClient(out error, useSsl, lmsDomain, licenseSettings[LmsLicenseSettingNames.BlackBoardUsername].ToString(), licenseSettings[LmsLicenseSettingNames.BlackBoardUserPassword].ToString());
    //        }

    //        error = "ASP.NET Session is expired";
    //        return null;
    //    }

    //    /// <summary>
    //    /// The initialize client.
    //    /// </summary>
    //    /// <param name="lmsDomain">
    //    /// The LMS domain.
    //    /// </param>
    //    /// <param name="client">
    //    /// The client.
    //    /// </param>
    //    /// <param name="error">
    //    /// The error.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="bool"/>.
    //    /// </returns>
    //    private bool InitializeClient(string lmsDomain, out WebserviceWrapper client, out string error)
    //    {
    //        client = new WebserviceWrapper(
    //            lmsDomain,
    //            VendorEgc,
    //            ProgramLti,
    //            TimeSpan.FromMinutes(1).Seconds,
    //            new CastleLoggerAdapter(_logger));

    //        if (HadError(client, out error))
    //        {
    //            return false;
    //        }

    //        client.initialize_v1();
    //        return !HadError(client, out error);
    //    }

    //    /// <summary>
    //    /// The get role.
    //    /// </summary>
    //    /// <param name="roleId">
    //    /// The role id.
    //    /// </param>
    //    /// <param name="availableRoles">
    //    /// The available roles.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="string"/>.
    //    /// </returns>
    //    private static string GetRole(string roleId, IEnumerable<CourseMembershipRoleVO> availableRoles)
    //    {
    //        var role =
    //            availableRoles.FirstOrDefault(x => x.roleIdentifier.Equals(roleId, StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : availableRoles.FirstOrDefault(x => x.roleIdentifier.Equals(roleId, StringComparison.OrdinalIgnoreCase)).courseRoleDescription
    //            // NOTE: we need to have original role name .ToLowerInvariant()
    //            .Trim(':')
    //            .Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
    //            .LastOrDefault();
    //        //.IfElse(x => x.Length <= 1, x => x.FirstOrDefault(), x => x.LastOrDefault());
    //        //return Inflector.Capitalize(role);

    //        return role;
    //    }

    //    private static string GetHost(string lmsDomain, bool useSsl)
    //    {
    //        Match match = portRegex.Match(lmsDomain);
    //        bool endsWithPort = match.Success;

    //        lmsDomain = lmsDomain.AddHttpProtocol(useSsl);
    //        lmsDomain = !endsWithPort ? (useSsl ? lmsDomain + ":443" : lmsDomain + ":80") : lmsDomain;

    //        return lmsDomain;
    //    }

    //    private static string FixHostFromString(string lmsDomain)
    //    {
    //        Match match = portRegex.Match(lmsDomain);
    //        bool endsWithPort = match.Success;
    //        var useSsl = lmsDomain.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase);
    //        lmsDomain = !endsWithPort ? (useSsl ? lmsDomain + ":443" : lmsDomain + ":80") : lmsDomain;
    //        return lmsDomain;
    //    }

    //    /// <summary>
    //    /// The login if necessary.
    //    /// </summary>
    //    /// <typeparam name="T">
    //    /// Any type
    //    /// </typeparam>
    //    /// <param name="client">
    //    /// The client.
    //    /// </param>
    //    /// <param name="action">
    //    /// The action.
    //    /// </param>
    //    /// <param name="lmsCompany">
    //    /// The company LMS.
    //    /// </param>
    //    /// <param name="error">
    //    /// The error.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="bool"/>.
    //    /// </returns>
    //    // ReSharper disable once UnusedMember.Local
    //    public T LoginIfNecessary<T>(ref WebserviceWrapper client, Func<WebserviceWrapper, T> action, Dictionary<string, object> licenseSettings, out string error)
    //    {
    //        error = null;
    //        client = client ?? BeginBatch(out error, licenseSettings);
    //        if (client != null)
    //        {
    //            return action(client);
    //        }

    //        return default(T);
    //    }

    //    private T LoginIfNecessary<T>(ref WebserviceWrapper client, Func<WebserviceWrapper, Tuple<T, string>> action, Dictionary<string, object> licenseSettings, out string error)
    //    {
    //        error = null;
    //        client = client ?? BeginBatch(out error, licenseSettings);
    //        if (client != null)
    //        {
    //            var result = action(client);
    //            error = result.Item2;

    //            if (!string.IsNullOrWhiteSpace(error))
    //                throw new InvalidOperationException("BlackBoard.LoginIfNecessary. Error: " + error);

    //            return result.Item1;
    //        }

    //        return default(T);
    //    }

    //    private static bool HadError(WebserviceWrapper ws, out string error)
    //    {
    //        error = null;
    //        if (ws == null)
    //        {
    //            error = "NULL webservicewrapper";
    //            return true;
    //        }

    //        string lastError = ws.getLastError();
    //        if (lastError != null)
    //        {
    //            error = lastError;
    //            return true;
    //        }

    //        return false;
    //    }

    //    public string[] CreateAnnouncement(string courseId, string userUuid, Dictionary<string, object> licenseSettings, string announcementTitle, string announcementMessage)
    //    {
    //        string error;
    //        var courseIdFixed = string.Format("_{0}_1", courseId);

    //        var client = BeginBatch(out error, licenseSettings);

    //        CourseMembershipWrapper membershipWS = client.getCourseMembershipWrapper();
    //        var membershipFilter = new MembershipFilter
    //        {
    //            filterType = 2,
    //            filterTypeSpecified = true,
    //            courseIds = new[] { courseIdFixed },
    //        };
    //        var enrollments = membershipWS.loadCourseMembership(courseIdFixed, membershipFilter);

    //        var userWS = client.getUserWrapper();
    //        var courseUsersFilter = new UserFilter
    //        {
    //            filterTypeSpecified = true,
    //            filterType = 2,
    //            id = enrollments.Select(x => x.userId).ToArray(),
    //        };
    //        var courseUsers = userWS.getUser(courseUsersFilter);
    //        var uuidString = "USER.UUID=" + userUuid;
    //        var user = courseUsers.FirstOrDefault(cu => cu != null
    //            && cu.expansionData != null
    //            && cu.expansionData.Any(ed => string.Compare(uuidString, ed, StringComparison.OrdinalIgnoreCase) == 0));

    //        if (user == null)
    //        {
    //            var adminRoles = new[] { "SYSTEM_ADMIN", "COURSE_CREATOR" };
    //            user = courseUsers.FirstOrDefault(x => x.systemRoles.Any(role => adminRoles.Contains(role)));
    //        }

    //        if (user != null)
    //        {
    //            var announcementVO = new AnnouncementVO
    //            {
    //                body = announcementMessage,
    //                creatorUserId = user.id,
    //                courseId = courseIdFixed,
    //                permanent = true,
    //                permanentSpecified = true,
    //                position = 1,
    //                positionSpecified = true,
    //                pushNotify = true,
    //                pushNotifySpecified = true,
    //                title = announcementTitle,
    //            };

    //            var annWS = client.getAnnouncementWrapper();
    //            var results = annWS.saveCourseAnnouncements(courseIdFixed, new[] { announcementVO });

    //            return results;
    //        }

    //        return null;
    //    }

    //    #endregion

    //}
}
