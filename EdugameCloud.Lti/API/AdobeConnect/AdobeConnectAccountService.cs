using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class AdobeConnectAccountService : IAdobeConnectAccountService
    {
        private readonly ILogger _logger;


        public AdobeConnectAccountService(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }


        public IAdobeConnectProxy GetProvider(ILmsLicense license, bool login = true)
        {
            var credentials = new UserCredentials(license.AcUsername, license.AcPassword);
            try
            {
                return GetProvider(license.AcServer, credentials, login);
            }
            catch (InvalidOperationException ex)
            {
                throw new Core.WarningMessageException("Adobe Connect Server credentials are not valid in the license. Please contact EGC administrator.", ex);
            }
        }

        public IAdobeConnectProxy GetProvider(string acDomain, UserCredentials credentials, bool login)
        {
            var apiUrl = new Uri(acDomain);

            var connectionDetails = new ConnectionDetails(apiUrl);
            string principalId = null;
            var provider = new AdobeConnectProvider(connectionDetails);
            if (login)
            {
                LoginResult result = provider.Login(credentials);
                if (!result.Success)
                {
                    _logger.Error($"AdobeConnectAccountService.GetProvider. Login failed to { acDomain }. Status: { result.Status.GetErrorInfo() }");
                    throw new InvalidOperationException($"Login to Adobe Connect failed. Status: { result.Status.GetErrorInfo() }");
                }
                principalId = result.User.UserId;
            }

            return new AdobeConnectProxy(provider, _logger, apiUrl);
        }
        

        public ACDetailsDTO GetAccountDetails(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, ICache cache)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            var item = CacheUtility.GetCachedItem<ACDetailsDTO>(cache, CachePolicies.Keys.AcDetails(provider.AdobeConnectRoot.ToString()), () =>
            {
                return IoC.Resolve<Esynctraining.AdobeConnect.IAdobeConnectAccountService>().GetAccountDetails(provider);
            });

            return item;
        }

        public IEnumerable<PrincipalReportDto> GetMeetingHostReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var group = provider.GetGroupsByType(PrincipalType.live_admins);
            if (group.Status.Code != StatusCodes.ok)
                throw new InvalidOperationException("AC.GetGroupsByType error");

            PrincipalCollectionResult usersResult = provider.GetGroupUsers(group.Values.First().PrincipalId);
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
            ILmsLicense lmsCompany,
            LtiParamDTO param,
            Principal registeredUser,
            string password,
            Esynctraining.AdobeConnect.IAdobeConnectProxy provider,
            bool updateAcUser = true)
        {
            if(registeredUser == null)
                throw new ArgumentNullException(nameof(registeredUser));

            string breezeToken = null;

            if (updateAcUser)
            {
                try
                {
                    var principalUpdateResult = provider.PrincipalUpdate(
                        new PrincipalSetup
                        {
                            PrincipalId = registeredUser.PrincipalId,
                            FirstName = param.PersonNameGiven,
                            LastName = param.PersonNameFamily,
                        }, true);
                }
                catch (AdobeConnectException ex)
                {
                    _logger.Error(ex.Status.GetErrorInfo());
                    throw new Core.WarningMessageException(
                        string.Format(
                            "Error has occured trying to access \"{0} {1}\" account in Adobe Connect. Please check that account used to access has sufficient permissions."
                            , param.PersonNameGiven
                            , param.PersonNameFamily));
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
                    string.Format("[LoginIntoAC Error] {0}. Login:{1}.  UserId:{2}. ConsumerKey:{3}",
                        resultByLogin.Status.GetErrorInfo(),
                        registeredUser.Login,
                        param.user_id,
                        param.oauth_consumer_key);
                _logger.Error(msg);
            }

            return breezeToken;
        }

        // TODO: USE Esynctraining.AdobeConnect.WebApi.Meeting !!!
        public IEnumerable<TemplateDto> GetSharedMeetingTemplates(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, ICache cache)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            return new MeetingTemplateService(_logger).GetSharedMeetingTemplates(provider, cache, () => CachePolicies.Keys.SharedMeetingTemplates(provider.AdobeConnectRoot.ToString(), ScoShortcutType.shared_meeting_templates.ToString()));
        }
        
    }

}