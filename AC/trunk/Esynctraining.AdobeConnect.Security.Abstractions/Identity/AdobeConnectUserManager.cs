using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Security.Abstractions.DomainValidation;
using Esynctraining.AdobeConnect.Security.Abstractions.PermissionProviders;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Security.Abstractions.Identity
{
    public sealed class AdobeConnectUserManager
    {
        private readonly IAcDomainValidator _acDomainValidator;
        private readonly IUserGroupPermissionProvider _userGroupPermissionProvider;
        private readonly IUserAuthorizationProvider _userAuthorizationProvider;
        private readonly ILogger _logger;


        public AdobeConnectUserManager(
            IAcDomainValidator acDomainValidator,
            IUserGroupPermissionProvider userGroupPermissionProvider,
            //IUserStore<AdobeConnectUser> userStore, 
            ILogger logger,
            IUserAuthorizationProvider userAuthorizationProvider = null
            )
        {
            _acDomainValidator = acDomainValidator ?? throw new ArgumentNullException(nameof(acDomainValidator));
            _userGroupPermissionProvider = userGroupPermissionProvider ?? throw new ArgumentNullException(nameof(userGroupPermissionProvider));
            _userAuthorizationProvider = userAuthorizationProvider;
            _logger = logger;
        }


        public Task<AdobeConnectUser> FindAsync(string userName, string password)
        {
            string[] parts = userName.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string companyToken = parts[0];
            string acDomain = parts[1];
            string acLogin = parts[2];

            if (!_acDomainValidator.IsValid(companyToken, acDomain))
            {
                _logger?.Warn($"[UserManager.FindAsync] AC domain is not valid for companyToken. AcDomain={acDomain}");
                return null;
            }

            string sessionToken;
            var connectionDetails = new ConnectionDetails(new Uri(acDomain));
            var provider = new AdobeConnectProvider(connectionDetails);
            UserInfo acPrincipal = TryLogin(provider, new AdobeConnectAccess(new Uri(acDomain), acLogin, password), out sessionToken);
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

            return Task.FromResult(applicationUser);
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

    }

}
