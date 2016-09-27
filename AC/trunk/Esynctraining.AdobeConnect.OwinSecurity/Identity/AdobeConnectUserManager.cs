using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ConnectExtensions.Services.Client;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.OwinSecurity.PermissionProviders;
using Esynctraining.Core.Logging;
using Microsoft.AspNet.Identity;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public sealed class AdobeConnectUserManager : UserManager<AdobeConnectUser>
    {
        private IUserGroupPermissionProvider _userGroupPermissionProvider;
        private ILogger _logger;

        public AdobeConnectUserManager() : this(null, null, null)
        {

        }

        public AdobeConnectUserManager(IUserGroupPermissionProvider userGroupPermissionProvider,
            IUserStore<AdobeConnectUser> userStore, ILogger logger) : base(userStore ?? new EdugameCloudUserStore<AdobeConnectUser>())
        {
            //We can retrieve Old System Hash Password and can encypt or decrypt old password using custom approach. 
            //When we want to reuse old system password as it would be difficult for all users to initiate pwd change as per Idnetity Core hashing. 
            //this.PasswordHasher = new EdugameCloudPasswordHasher();
            _userGroupPermissionProvider = userGroupPermissionProvider ?? new DefaultUserGroupPermissionProvider();
            _logger = logger;
        }

        public override System.Threading.Tasks.Task<AdobeConnectUser> FindAsync(string userName, string password)
        {
            Task<AdobeConnectUser> taskInvoke = Task.Run(async () =>
            {
                string[] parts = userName.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                string companyToken = parts[0];
                string acDomain = parts[1];
                string acLogin = parts[2];

                var licenseProductId = int.Parse(ConfigurationManager.AppSettings["LicenseProductId"] as string);

                string[] acDomains = licenseProductId == 48 // NOTE: hardcoded in DB MP4 product
                    ? new CompanySubscriptionServiceProxy().GetAdobeConnectDomainsByCompanyToken(companyToken).Result
                    : new PublicLicenseServiceProxy().GetAdobeConnectDomainsByCompanyToken(companyToken, licenseProductId).Result;

                if (!acDomains.Any(x => x.Equals(acDomain, StringComparison.OrdinalIgnoreCase)))
                {
                    // TODO: add to log that AC domain is not valid for companyToken!!
                    //                    _logger?.Warn($"[UserManager.FindAsync] AC domain is not valid for companyToken. AcDomain={acDomain}");
                    return null;
                }

                string sessionToken;
                UserInfo acPrincipal = TryLogin(new AdobeConnectAccess(acDomain, acLogin, password), out sessionToken);
//                _logger?.Info($"[UserManager.FindAsync] ACSession={sessionToken}");

                if (acPrincipal == null)
                {
//                    _logger?.Warn($"[UserManager.FindAsync] Principal not found. AcDomain={acDomain}, AcLogin={acLogin}");
                    return null;
                }

                var applicationUser = new AdobeConnectUser
                {
                    Id = acPrincipal.UserId,
                    UserName = acLogin,

                    CompanyToken = companyToken,
                    AcDomain = acDomain,
                    AcSessionToken = sessionToken,
                };
                var store = Store as IEdugameCloudUserStore<AdobeConnectUser>;
                if (store != null)
                {
                    var user = await store.FindByPrincipalIdAndCompanyTokenAndAcDomainAsync(applicationUser.Id, companyToken,
                        acDomain);
                    if (user == null)
                    {
                        //_logger?.Warn($"[UserManager.FindAsync] UserStore.CreateAsync. PrincipalId={applicationUser.Id}");
                        await store.CreateAsync(applicationUser, password);
                    }
                }

                return applicationUser;

            });
            return taskInvoke;
        }

        private UserInfo TryLogin(IAdobeConnectAccess credentials, out string sessionToken)
        {
            string apiUrl = credentials.Domain + "/api/xml";
            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);

            provider.Logout();
            LoginResult result = provider.Login(new UserCredentials(credentials.Login, credentials.Password));
            if (!result.Success)
            {
//                _logger?.Error(
//                    $"[UserManager.TryLogin] Login failed. Login={credentials.Login}, Status={result.Status.GetErrorInfo()}");
                sessionToken = null;
                return null;
            }

            sessionToken = result.Status.SessionInfo;
//            _logger?.Info($"[UserManager.TryLogin] Success. Login={credentials.Login}, sessionToken={sessionToken}");

            return _userGroupPermissionProvider.UserHasGroupPermission(provider, result.User) ? result.User : null;
        }

        public async Task<AdobeConnectUser> RefreshSession(string userId, string companyToken, string domain,
            string userName)
        {
//            _logger?.Info($"[UserManager.RefreshSession] PrincipalId={userId}, Domain={domain}");

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException(nameof(userId));
            if (string.IsNullOrEmpty(companyToken))
                throw new ArgumentException(nameof(companyToken));
            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException(nameof(domain));
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException(nameof(userName));

            var store = Store as IEdugameCloudUserStore<AdobeConnectUser>;
            if (store != null)
            {
                var login = $"{companyToken}|{domain}|{userName}";
                var password = await store.RetrievePassword(userId, companyToken, domain);
                if (!string.IsNullOrEmpty(password))
                {
                    return await FindAsync(login, password);
                }
            }

            return null;
        }
    }
}
