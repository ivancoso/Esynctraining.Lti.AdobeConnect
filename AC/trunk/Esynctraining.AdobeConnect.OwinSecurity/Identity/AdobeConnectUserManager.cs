using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.OwinSecurity.DomainValidation;
using Esynctraining.AdobeConnect.OwinSecurity.PermissionProviders;
using Esynctraining.Core.Logging;
using Microsoft.AspNet.Identity;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public sealed class AdobeConnectUserManager : UserManager<AdobeConnectUser>
    {
        private readonly IAcDomainValidator _acDomainValidator;
        private readonly IUserGroupPermissionProvider _userGroupPermissionProvider;
        private readonly IUserAuthorizationProvider _userAuthorizationProvider;
        private readonly ILogger _logger;


        //public AdobeConnectUserManager() 
        //    : this(new DefaultUserGroupPermissionProvider(), new EdugameCloudUserStore<AdobeConnectUser>(), null)
        //{
        //}

        public AdobeConnectUserManager(
            IAcDomainValidator acDomainValidator,
            IUserGroupPermissionProvider userGroupPermissionProvider,
            IUserStore<AdobeConnectUser> userStore, 
            ILogger logger,
            IUserAuthorizationProvider userAuthorizationProvider = null
            )
            : base(userStore)
        {
            if (acDomainValidator == null)
                throw new ArgumentNullException(nameof(acDomainValidator));
            if (userGroupPermissionProvider == null)
                throw new ArgumentNullException(nameof(userGroupPermissionProvider));
            if (userStore == null)
                throw new ArgumentNullException(nameof(userStore));

            //We can retrieve Old System Hash Password and can encypt or decrypt old password using custom approach. 
            //When we want to reuse old system password as it would be difficult for all users to initiate pwd change as per Idnetity Core hashing. 
            //this.PasswordHasher = new EdugameCloudPasswordHasher();
            _acDomainValidator = acDomainValidator;
            _userGroupPermissionProvider = userGroupPermissionProvider;
            _userAuthorizationProvider = userAuthorizationProvider;
            _logger = logger;
        }


        //public static IUserGroupPermissionProvider DefaultUserGroupPermissionProvider()
        //{
        //    return new DefaultUserGroupPermissionProvider();
        //}

        public override Task<AdobeConnectUser> FindAsync(string userName, string password)
        {
            Task<AdobeConnectUser> taskInvoke = Task.Run(async () =>
            {
                string[] parts = userName.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                string companyToken = parts[0];
                string acDomain = parts[1];
                string acLogin = parts[2];

                if (!_acDomainValidator.IsValid(companyToken, acDomain))
                {
                    _logger?.Warn($"[UserManager.FindAsync] AC domain is not valid for companyToken. AcDomain={acDomain}");
                    return null;
                }

                string sessionToken;
                string apiUrl = acDomain + "/api/xml";
                var connectionDetails = new ConnectionDetails(apiUrl);
                var provider = new AdobeConnectProvider(connectionDetails);
                UserInfo acPrincipal = TryLogin(provider, new AdobeConnectAccess(acDomain, acLogin, password), out sessionToken);
                _logger?.Info($"[UserManager.FindAsync] ACSession={sessionToken}");

                if (acPrincipal == null)
                {
                    _logger?.Warn($"[UserManager.FindAsync] Principal not found. AcDomain={acDomain}, AcLogin={acLogin}");
                    return null;
                }

                var roles = new List<string>();
                if (_userAuthorizationProvider != null)
                {
                    roles.AddRange(_userAuthorizationProvider.GetUserPermissions(provider, acPrincipal));
                }

                var applicationUser = new AdobeConnectUser
                {
                    Id = acPrincipal.UserId,
                    UserName = acLogin,

                    CompanyToken = companyToken,
                    AcDomain = acDomain,
                    AcSessionToken = sessionToken,
                    Roles = roles
                };
                var store = Store as IEdugameCloudUserStore<AdobeConnectUser>;
                if (store != null)
                {
                    var user = await store.FindByPrincipalIdAndCompanyTokenAndAcDomainAsync(applicationUser.Id, companyToken,
                        acDomain);
                    if (user == null)
                    {
                        _logger?.Warn($"[UserManager.FindAsync] UserStore.CreateAsync. PrincipalId={applicationUser.Id}");
                        await store.CreateAsync(applicationUser, password);
                    }
                }

                return applicationUser;

            });
            return taskInvoke;
        }

        private UserInfo TryLogin(AdobeConnectProvider provider, IAdobeConnectAccess credentials, out string sessionToken)
        {
            provider.Logout();
            LoginResult result = provider.Login(new UserCredentials(credentials.Login, credentials.Password));
            if (!result.Success)
            {
                _logger?.Error(
                    $"[UserManager.TryLogin] Login failed. Login={credentials.Login}, Status={result.Status.GetErrorInfo()}");
                sessionToken = null;
                return null;
            }

            sessionToken = result.Status.SessionInfo;
            _logger?.Info($"[UserManager.TryLogin] Success. Login={credentials.Login}, sessionToken={sessionToken}");

            return _userGroupPermissionProvider.UserHasGroupPermission(provider, result.User) ? result.User : null;
        }

        public async Task<AdobeConnectUser> RefreshSession(string userId, string companyToken, string domain,
            string userName)
        {
            _logger?.Info($"[UserManager.RefreshSession] PrincipalId={userId}, Domain={domain}");

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
