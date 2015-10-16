using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class AdobeConnectAccountService : IAdobeConnectAccountService
    {
        private readonly ILogger _logger;

        public AdobeConnectAccountService(ILogger logger)
        {
            _logger = logger;
        }

        public IAdobeConnectProxy GetProvider(ILmsLicense license, bool login = true)
        {
            var credentials = new UserCredentials(license.AcUsername, license.AcPassword);
            return GetProvider(license, credentials, login);
        }

        public IAdobeConnectProxy GetProvider(ILmsLicense license, UserCredentials credentials, bool login)
        {
            string apiUrl = license.AcServer + "/api/xml";

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
            var provider = new AdobeConnectProvider(connectionDetails);
            if (login)
            {
                LoginResult result = provider.Login(credentials);
                if (!result.Success)
                {
                    _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status.Code:Status.SubCode = " + result.Status.Code.ToString() + ":" + result.Status.SubCode.ToString());
                    throw new InvalidOperationException("Login to Adobe Connect failed. Status.Code:Status.SubCode = " + result.Status.Code.ToString() + ":" + result.Status.SubCode.ToString());
                }
            }

            return new AdobeConnectProxy(provider, IoC.Resolve<ILogger>(), apiUrl);
        }


        public ACPasswordPoliciesDTO GetPasswordPolicies(IAdobeConnectProxy provider, ICache cache)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (cache == null)
                throw new ArgumentNullException("cache");
            
            var item = CacheUtility.GetCachedItem<ACPasswordPoliciesDTO>(cache, CachePolicies.Keys.PasswordPolicies(provider.ApiUrl), () =>
            {
                StatusInfo status;
                UserInfo usr = provider.GetUserInfo(out status);

                if (status.Code != StatusCodes.ok)
                {
                    _logger.ErrorFormat("GetPasswordPolicies.GetUserInfo. AC error. Code:{0}.SubCode:{1}.", status.Code, status.SubCode);
                    return null;
                }

                if (usr.AccountId.HasValue)
                {
                    FieldCollectionResult fields = provider.GetAclFields(usr.AccountId.Value);

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

                    return new ACPasswordPoliciesDTO
                    {
                        passwordRequiresDigit = passwordRequiresDigit,
                        passwordRequiresCapitalLetter = passwordRequiresCapitalLetter,
                        passwordRequiresSpecialChars = passwordRequiresSpecialChars,
                        passwordMinLength = passwordMinLength,
                        passwordMaxLength = passwordMaxLength,
                    };
                }

                _logger.Error("GetPasswordPolicies. Account is NULL");
                return null;

            });

            return item;
        }

        public IEnumerable<PrincipalReportDto> GetMeetingHostReport(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            var group = provider.GetGroupsByType("live-admins");
            if (group.Item1.Code != StatusCodes.ok)
                throw new InvalidOperationException("AC.GetGroupsByType error");

            PrincipalCollectionResult usersResult = provider.GetGroupUsers(group.Item2.First().PrincipalId);
            if (usersResult.Status.Code != StatusCodes.ok)
                throw new InvalidOperationException("AC.GetGroupUsers error");

            var result = new List<PrincipalReportDto>();
            foreach (Principal user in usersResult.Values)
            {
                var item = new PrincipalReportDto 
                {
                    Principal = PrincipalDto.Build(user),
                };

                TransactionCollectionResult trxResult = provider.ReportMeetingTransactionsForPrincipal(user.PrincipalId, startIndex: 0, limit: 1);

                if (trxResult.Status.Code != StatusCodes.ok)
                    throw new InvalidOperationException("AC.ReportMeetingTransactionsForPrincipal error");

                TransactionInfo trx = trxResult.Values.FirstOrDefault();

                if (trx != null)
                    item.LastTransaction = TransactionInfoDto.Build(trx);

                result.Add(item);
            }

            return result.AsReadOnly();
        }

        public string LoginIntoAC(
            LmsCompany lmsCompany,
            LtiParamDTO param,
            Principal registeredUser,
            string password,
            IAdobeConnectProxy provider,
            bool updateAcUser = true)
        {
            if(registeredUser == null)
                throw new ArgumentNullException("registeredUser");

            string breezeToken = null;

            if (updateAcUser)
            {
                try
                {
                    var principalUpdateResult = provider.PrincipalUpdate(
                        new PrincipalSetup
                        {
                            PrincipalId = registeredUser.PrincipalId,
                            FirstName = param.lis_person_name_given,
                            LastName = param.lis_person_name_family,
                        }, true);
                }
                catch
                {
                    throw new WarningMessageException(
                        string.Format(
                            "Error has occured trying to access \"{0} {1}\" account in Adobe Connect. Please check that account used to access has sufficient permissions."
                            , param.lis_person_name_given
                            , param.lis_person_name_family));
                }
            }
            var userProvider = this.GetProvider(lmsCompany, false); // separate provider for user not to lose admin logging in

            LoginResult resultByLogin = userProvider.Login(new UserCredentials(registeredUser.Login, password));
            if (resultByLogin.Success)
            {
                breezeToken = resultByLogin.Status.SessionInfo;
            }
            else
            {
                string msg =
                    string.Format("[LoginIntoAC Error] {0}. Login:{1}. Password:{2}. UserId:{3}. ConsumerKey:{4}",
                        resultByLogin.Status.GetErrorInfo(),
                        registeredUser.Login,
                        password,
                        param.user_id,
                        param.oauth_consumer_key);
                _logger.Error(msg);
            }

            return breezeToken;
        }

        /// <summary>
        /// The get templates.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="templateFolder">
        /// The template folder.
        /// </param>
        /// <returns>
        /// The <see cref="List{TemplateDTO}"/>.
        /// </returns>
        public IEnumerable<TemplateDTO> GetTemplates(IAdobeConnectProxy provider, string templateFolder)
        {
            ScoContentCollectionResult result = provider.GetContentsByScoId(templateFolder);
            if (result.Values == null)
            {
                return new List<TemplateDTO>();
            }

            return result.Values.Select(v => new TemplateDTO { id = v.ScoId, name = v.Name }).ToList();
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