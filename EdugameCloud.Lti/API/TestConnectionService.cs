using System;
using System.Linq;
using BbWsClient;
using Castle.Core.Logging;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.BrainHoney;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API
{
    public sealed class TestConnectionService
    {
        private const string OkMessage = "Connected successfully";


        private readonly ILogger logger;


        public TestConnectionService(ILogger logger)
        {
            this.logger = logger;
        }

        #region Properties

        private IBrainHoneyApi DlapAPI
        {
            get { return IoC.Resolve<IBrainHoneyApi>(); }
        }

        private IMoodleApi MoodleAPI
        {
            get { return IoC.Resolve<IMoodleApi>(); }
        }

        private IBlackBoardApi SoapAPI
        {
            get { return IoC.Resolve<IBlackBoardApi>(); }
        }

        #endregion

        public ConnectionInfoDTO TestConnection(ConnectionTestDTO test)
        {
            bool success = false;
            string info = string.Empty;

            switch (test.type.ToLowerInvariant())
            {
                case "ac":
                    bool loginSameAsEmail;
                    success = this.TestACConnection(test, out info, out loginSameAsEmail);
                    break;
                case "brainhoney":
                    success = this.TestBrainHoneyConnection(test, out info);
                    break;
                case "blackboard":
                    success = this.TestBlackBoardConnection(test, out info);
                    break;
                case "moodle":
                    success = this.TestMoodleConnection(test, out info);
                    break;
                case "sakai":
                    success = this.TestSakaiConnection(test, out info);
                    break;
                case "canvas":
                    success = TestCanvasConnection(test, out info);
                    break;
                case "brightspace":
                    success = TestBrightspaceConnection(test, out info);
                    break;
            }

            return new ConnectionInfoDTO { status = success ? OkMessage : "Failed to connect", info = info };
        }

        #region Methods

        private bool TestMoodleConnection(ConnectionTestDTO test, out string info)
        {
            if (!test.domain.StartsWithProtocol())
            {
                info = "Domain url should start with http:// or https://";
                return false;
            }

            return this.MoodleAPI.LoginAndCheckSession(out info,
                test.domain.IsSSL(),
                test.domain.RemoveHttpProtocolAndTrailingSlash(),
                test.login,
                test.password);
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

        private bool TestBrightspaceConnection(ConnectionTestDTO test, out string info)
        {
            info = "Not supported";
            return true;
        }

        private bool TestBrainHoneyConnection(ConnectionTestDTO test, out string info)
        {
            if (!test.domain.StartsWithProtocol())
            {
                info = "Domain url should start with http:// or https://";
                return false;
            }

            return this.DlapAPI.LoginAndCheckSession(out info, test.domain.RemoveHttpProtocolAndTrailingSlash(), test.login, test.password);
        }

        private bool TestBlackBoardConnection(ConnectionTestDTO test, out string info)
        {
            if (!test.domain.StartsWithProtocol())
            {
                info = "Domain url should start with http:// or https://";
                return false;
            }

            WebserviceWrapper session = test.enableProxyToolMode
                ? this.SoapAPI.LoginToolAndCreateAClient(
                    out info,
                    test.domain.IsSSL(),
                    test.domain,
                    test.password)

                : this.SoapAPI.LoginUserAndCreateAClient(
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

        public bool TestACConnection(ConnectionTestDTO test, out string info, out bool loginSameAsEmail)
        {
            loginSameAsEmail = false;
            info = string.Empty;
            var domainUrl = test.domain.ToLowerInvariant();
            if (!domainUrl.StartsWithProtocol())
            {
                info = "Adobe Connect Domain url should start with http or https";
                return false;
            }

            var fixedUrl = domainUrl.EndsWith("/") ? domainUrl : string.Format("{0}/", domainUrl);
            fixedUrl = fixedUrl.EndsWith("api/xml/") ? fixedUrl : string.Format("{0}api/xml", fixedUrl);

            var provider = new AdobeConnectProvider(new ConnectionDetails { ServiceUrl = fixedUrl });

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

            StatusInfo status;
            UserInfo usr = provider.GetUserInfo(out status);

            if (status.Code != StatusCodes.ok)
            {
                logger.ErrorFormat("GetPasswordPolicies.GetUserInfo. AC error. {0}.", status.GetErrorInfo());
                info = status.GetErrorInfo();
                return false;
            }

            if ((usr != null) && usr.AccountId.HasValue)
            {
                FieldCollectionResult fields = provider.GetAclFields(usr.AccountId.Value);

                if (!fields.Success)
                {
                    logger.ErrorFormat("GetPasswordPolicies.GetAclFields. AC error. {0}.", fields.Status.GetErrorInfo());
                    info = fields.Status.GetErrorInfo();
                    return false;
                }

                string setting = GetField(fields, "login-same-as-email");
                loginSameAsEmail = string.IsNullOrEmpty(setting) || "YES".Equals(setting, StringComparison.OrdinalIgnoreCase);
                return true;
            }

            logger.Error("GetPasswordPolicies. Account is NULL. Check Adobe Connect account permissions. Admin account expected.");
            info = "Check Adobe Connect account permissions. Admin account expected.";
            return false;
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
