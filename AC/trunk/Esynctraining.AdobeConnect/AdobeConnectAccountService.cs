using System;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect
{
    public sealed class AdobeConnectAccountService : IAdobeConnectAccountService
    {
        private readonly ILogger _logger;


        public AdobeConnectAccountService(ILogger logger)
        {
            _logger = logger;
        }


        public IAdobeConnectProxy GetProvider(IAdobeConnectAccess credentials, bool login)
        {
            string apiUrl = credentials.Domain + "/api/xml";

            var connectionDetails = new ConnectionDetails
            {
                ServiceUrl = apiUrl,
                EventMaxParticipants = 10,
                Proxy =
                new ProxyCredentials
                {
                    Domain = string.Empty,
                    Login = string.Empty,
                    Password = string.Empty,
                    Url = string.Empty,
                },
            };
            string principalId = null;
            var provider = new AdobeConnectProvider(connectionDetails);
            if (login)
            {
                LoginResult result = provider.Login(new UserCredentials(credentials.Login, credentials.Password));
                if (!result.Success)
                {
                    _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                    throw new InvalidOperationException("Login to Adobe Connect failed. Status = " + result.Status.GetErrorInfo());
                }
                principalId = result.User.UserId;
            }

            return new AdobeConnectProxy(provider, _logger, apiUrl);
        }

        public IAdobeConnectProxy GetProvider2(IAdobeConnectAccess2 credentials)
        {
            string apiUrl = credentials.Domain + "/api/xml";

            var connectionDetails = new ConnectionDetails
            {
                ServiceUrl = apiUrl,
                EventMaxParticipants = 10,
            };
            string principalId = null;
            var provider = new AdobeConnectProvider(connectionDetails);
            {
                LoginResult result = provider.LoginWithSessionId(credentials.SessionToken);
                if (!result.Success)
                {
                    _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                    throw new InvalidOperationException("Login to Adobe Connect failed. Status = " + result.Status.GetErrorInfo());
                }
                principalId = result.User.UserId;
            }

            return new AdobeConnectProxy(provider, _logger, apiUrl);
        }

        public ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            CommonInfoResult commonInfo = provider.GetCommonInfo();

            if (!commonInfo.Success)
            {
                _logger.ErrorFormat("GetPasswordPolicies.GetUserInfo. AC error:{0}.", commonInfo.Status.GetErrorInfo());
                return null;
            }

            if (commonInfo.CommonInfo.AccountId.HasValue)
            {
                FieldCollectionResult fields = provider.GetAclFields(commonInfo.CommonInfo.AccountId.Value);

                if (fields.Status.Code != StatusCodes.ok)
                {
                    _logger.ErrorFormat("GetPasswordPolicies.GetAclFields. AC error. Code:{0}.SubCode:{1}.", fields.Status.Code, fields.Status.SubCode);
                    return null;
                }

                //bool loginSameAsEmail = fields.Values.First(x => x.FieldId == "login-same-as-email").Value.Equals("YES", StringComparison.OrdinalIgnoreCase);
                bool passwordRequiresDigit = "YES".Equals(GetField(fields, "password-requires-digit"), StringComparison.OrdinalIgnoreCase);
                bool passwordRequiresCapitalLetter = "YES".Equals(GetField(fields, "password-requires-capital-letter"), StringComparison.OrdinalIgnoreCase);
                string passwordRequiresSpecialChars = GetField(fields, "password-requires-special-chars");

                int passwordMinLength = int.Parse(GetField(fields, "password-min-length") ?? "4");
                int passwordMaxLength = int.Parse(GetField(fields, "password-max-length") ?? "32");

                return new ACDetailsDTO
                {
                    Version = commonInfo.CommonInfo.Version,
                    TimeZoneShiftMinutes = commonInfo.CommonInfo.GetTimeZoneShiftMinutes(),

                    PasswordPolicies = new ACPasswordPoliciesDTO
                    {
                        PasswordRequiresDigit = passwordRequiresDigit,
                        PasswordRequiresCapitalLetter = passwordRequiresCapitalLetter,
                        PasswordRequiresSpecialChars = passwordRequiresSpecialChars,
                        PasswordMinLength = passwordMinLength,
                        PasswordMaxLength = passwordMaxLength,
                    },
                };
            }

            _logger.Error("GetPasswordPolicies. Account is NULL");
            return null;

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

    }

}
