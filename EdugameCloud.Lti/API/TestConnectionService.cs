using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.API.Haiku;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.API.Schoology;

namespace EdugameCloud.Lti.API
{
    public sealed class TestConnectionService
    {
        private const string OkMessage = "Connected successfully";


        private readonly ILogger _logger;
        private readonly ISchoologyRestApiClient _schoologyClient;
        private readonly IAgilixBuzzApi _buzzClient;
        private readonly IMoodleApi _moodleClient;
        private readonly IBlackBoardApi _blackboardClient;
        private readonly IHaikuRestApiClient _haikuClient;

        public TestConnectionService(ILogger logger, IAgilixBuzzApi buzzClient,
            IMoodleApi moodleClient, IBlackBoardApi blackboardClient,
            IHaikuRestApiClient haikuClient, ISchoologyRestApiClient schoologyClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schoologyClient = schoologyClient ?? throw new ArgumentNullException(nameof(schoologyClient));
            _buzzClient = buzzClient ?? throw new ArgumentNullException(nameof(buzzClient));
            _moodleClient = moodleClient ?? throw new ArgumentNullException(nameof(moodleClient));
            _blackboardClient = blackboardClient ?? throw new ArgumentNullException(nameof(blackboardClient));
            _haikuClient = haikuClient ?? throw new ArgumentNullException(nameof(haikuClient));
        }

        public ConnectionInfoDTO TestConnection(ConnectionTestDTO test)
        {
            bool success = false;
            string info = string.Empty;

            switch (test.type.ToLowerInvariant())
            {
                case "ac":
                    bool loginSameAsEmail;
                    success = TestACConnection(test, out info, out loginSameAsEmail);
                    break;
                case LmsProviderNames.AgilixBuzz:
                    success = TestAgilixBuzzConnection(test, out info);
                    break;
                case LmsProviderNames.Blackboard:
                    success = TestBlackBoardConnection(test, out info);
                    break;
                case LmsProviderNames.Moodle:
                    var tupleResult = TestMoodleConnection(test).Result;
                    success = tupleResult.result;
                    info = tupleResult.info;
                    break;
                case LmsProviderNames.Sakai:
                    success = TestSakaiConnection(test, out info);
                    break;
                case LmsProviderNames.Canvas:
                    success = TestCanvasConnection(test, out info);
                    break;
                case LmsProviderNames.Bridge:
                    success = TestBridgeConnection(test, out info);
                    break;
                case LmsProviderNames.Brightspace:
                    success = TestBrightspaceConnection(test, out info);
                    break;
                case LmsProviderNames.Schoology:
                    success = TestSchoologyConnection(test, out info);
                    break;
                case LmsProviderNames.Haiku:
                    success = TestHaikuConnection(test, out info);
                    break;
            }

            return new ConnectionInfoDTO { status = success ? OkMessage : "Failed to connect", info = info };
        }

        #region Methods

        private async Task<(bool result, string info)> TestMoodleConnection(ConnectionTestDTO test)
        {
            string info;
            if (!TestDomainFormat(test, out info))
                return (false, info);

            var tuple = await _moodleClient.LoginAndCheckSession(
                test.domain.IsSSL(),
                test.domain.RemoveHttpProtocolAndTrailingSlash(),
                test.login,
                test.password);

            return (tuple.result, info);
        }

        private bool TestSakaiConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not yet implemented";
            return true;
        }

        private bool TestCanvasConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not supported";
            return true;
        }

        private bool TestBridgeConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not supported";
            return true;
        }

        private bool TestBrightspaceConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not supported";
            return true;
        }

        private bool TestSchoologyConnection(ConnectionTestDTO test, out string info)
        {
            if (!TestDomainFormat(test, out info))
                return false;

            try
            {
                Task<dynamic> task = Task.Run<dynamic>(async () => await _schoologyClient.GetRestCall<dynamic>(test.login, test.password, "courses"));
                var courses = task.Result;
                return courses != null;
            }
            catch (AggregateException ex)
            {
                _logger.Error("[TestSchoologyConnection]", ex);
                if (ex.InnerExceptions.First() is HttpRequestException reqEx)
                {
                    info = reqEx.Message;
                }
                return false;
            }
        }

        private bool TestHaikuConnection(ConnectionTestDTO test, out string info)
        {
            if (!TestDomainFormat(test, out info))
                return false;

            if (!Task.Run(() => _haikuClient.TestOauthAsync(test.domain, test.consumerKey, test.consumerSecret, test.token, test.tokenSecret)).Result)
            {
                info = "Can't connect.";

                return false;
            }

            return true;
        }

        private bool TestAgilixBuzzConnection(ConnectionTestDTO test, out string info)
        {
            if (!TestDomainFormat(test, out info))
                return false;

            bool result = Task.Run(async () => await _buzzClient.LoginAndCheckSessionAsync(test.domain.RemoveHttpProtocolAndTrailingSlash(), test.login, test.password)).Result;

            return result;
        }

        private bool TestBlackBoardConnection(ConnectionTestDTO test, out string info)
        {
            if (!TestDomainFormat(test, out info))
                return false;

            var session = test.enableProxyToolMode
                ? _blackboardClient.LoginToolAndCreateAClient(
                    out info,
                    test.domain.IsSSL(),
                    test.domain,
                    test.password)

                : _blackboardClient.LoginUserAndCreateAClient(
                    out info,
                    test.domain.IsSSL(),
                    test.domain,
                    test.login,
                    test.password);

            bool success = session != null;
            if (session != null)
                session.logout();

            return success;
        }

        public bool TestACConnection(ConnectionTestDTO test, out string info, 
            out bool loginSameAsEmail)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
            
            loginSameAsEmail = false;
            //sharedTemplatesFolderScoId = null;
            info = string.Empty;

            if (string.IsNullOrWhiteSpace(test.password))
            {
                info = "Password is required";
                return false;
            }

            if (!TestDomainFormat(test, out info))
                return false;

            var provider = new AdobeConnectProvider(new ConnectionDetails(new Uri(test.domain)));
            var result = provider.Login(new UserCredentials(test.login, test.password));

            if (!result.Success)
            {
                string error = FormatErrorFromAC(result);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    info = error;
                }

                return false;
            }
            
            CommonInfoResult commonInfo = provider.GetCommonInfo();

            if (!commonInfo.Success)
            {
                _logger.ErrorFormat("GetPasswordPolicies.GetUserInfo. AC error. {0}.", commonInfo.Status.GetErrorInfo());
                info = commonInfo.Status.GetErrorInfo();
                return false;
            }

            if (commonInfo.CommonInfo.AccountId.HasValue)
            {
                FieldCollectionResult fields = provider.GetAclFields(commonInfo.CommonInfo.AccountId.Value);

                if (!fields.Success)
                {
                    _logger.ErrorFormat("GetPasswordPolicies.GetAclFields. AC error. {0}.", fields.Status.GetErrorInfo());
                    info = fields.Status.GetErrorInfo();
                    return false;
                }

                //ScoContentCollectionResult sharedTemplates = provider.GetContentsByType("shared-meeting-templates");
                //if (!sharedTemplates.Success)
                //{
                //    logger.ErrorFormat("GetPasswordPolicies.get shared-meeting-templates. AC error. {0}.", sharedTemplates.Status.GetErrorInfo());
                //    info = sharedTemplates.Status.GetErrorInfo();
                //    return false;
                //}

                //sharedTemplatesFolderScoId = sharedTemplates.ScoId;

                string setting = GetField(fields, "login-same-as-email");
                loginSameAsEmail = string.IsNullOrEmpty(setting) || "YES".Equals(setting, StringComparison.OrdinalIgnoreCase);
                return true;
            }

            _logger.Error("GetPasswordPolicies. Account is NULL. Check Adobe Connect account permissions. Admin account expected.");
            info = "Check Adobe Connect account permissions. Admin account expected.";
            return false;
        }

        private bool TestDomainFormat(ConnectionTestDTO test, out string error)
        {
            if (!test.domain.StartsWithProtocol())
            {
                error = "Domain url should start with http:// or https://";
                return false;
            }

            if (test.domain.EndsWith("/"))
            {
                error = "Domain url should not end with '/'";
                return false;
            }

            Uri tmp;
            if (!Uri.TryCreate(test.domain, UriKind.Absolute, out tmp))
            {
                error = "Invalid domain format";
                return false;
            }

            error = null;
            return true;
        }

        private static string GetField(FieldCollectionResult value, string fieldName)
        {
            Field field = value.Values.FirstOrDefault(x => x.FieldId == fieldName);
            if (field == null)
            {
                return null;
            }

            return field.Value;
        }

        private static string FormatErrorFromAC(ResultBase res)
        {
            if (res != null && res.Status != null)
            {
                string field = res.Status.InvalidField;
                Esynctraining.AC.Provider.Entities.StatusCodes errorCode = res.Status.Code;
                Esynctraining.AC.Provider.Entities.StatusSubCodes errorSubCode = res.Status.SubCode;
                string message;
                if (field == "login"
                    && errorCode == Esynctraining.AC.Provider.Entities.StatusCodes.invalid
                    && errorSubCode == Esynctraining.AC.Provider.Entities.StatusSubCodes.duplicate)
                {
                    message = "User already exists in Adobe Connect.";
                }
                else if (res is LoginResult
                    && !res.Success
                    && string.IsNullOrWhiteSpace(field)
                    && errorCode == Esynctraining.AC.Provider.Entities.StatusCodes.no_data)
                {
                    message = "Login failed";
                }
                else if (res.Status.UnderlyingExceptionInfo != null)
                {
                    message = res.Status.UnderlyingExceptionInfo.Message;
                }
                else
                {
                    message = string.Format(
                        "Adobe Connect error: {0}{1}{2}",
                        field,
                        errorCode == Esynctraining.AC.Provider.Entities.StatusCodes.not_set ? string.Empty : " is " + errorCode,
                        errorSubCode == Esynctraining.AC.Provider.Entities.StatusSubCodes.not_set
                            ? string.Empty
                            : string.Format(" (reason : {0})", errorSubCode));
                }

                return message;
            }

            return null;
        }

        #endregion

    }

}
