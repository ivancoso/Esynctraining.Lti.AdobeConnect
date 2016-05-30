using System;
using System.Linq;
using System.Threading.Tasks;
using ConnectExtensions.Services.Client;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Microsoft.AspNet.Identity;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public sealed class AdobeConnectUserManager : UserManager<AdobeConnectUser>
    {
        private IUserGroupPermissionProvider _userGroupPermissionProvider;

        public AdobeConnectUserManager() : this(null, null)
        {
            
        }

        public AdobeConnectUserManager(IUserGroupPermissionProvider userGroupPermissionProvider, IUserStore<AdobeConnectUser> userStore) : base(userStore ?? new EdugameCloudUserStore<AdobeConnectUser>())
        {
            //We can retrieve Old System Hash Password and can encypt or decrypt old password using custom approach. 
            //When we want to reuse old system password as it would be difficult for all users to initiate pwd change as per Idnetity Core hashing. 
            //this.PasswordHasher = new EdugameCloudPasswordHasher();
            _userGroupPermissionProvider = userGroupPermissionProvider ?? new DefaultUserGroupPermissionProvider();
        }

        public override System.Threading.Tasks.Task<AdobeConnectUser> FindAsync(string userName, string password)
        {
            Task<AdobeConnectUser> taskInvoke = Task<AdobeConnectUser>.Factory.StartNew(() =>
            {
                string[] parts = userName.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string companyToken = parts[0];
                string acDomain = parts[1];
                string acLogin = parts[2];

                string[] acDomains = new CompanySubscriptionServiceProxy().GetAdobeConnectDomainsByCompanyToken(companyToken).Result;
                if (!acDomains.Any(x => x.Equals(acDomain, StringComparison.OrdinalIgnoreCase)))
                {
                    // TODO: add to log that AC domain is not valid for companyToken!!

                    return null;
                }

                string sessionToken;
                UserInfo acPrincipal = TryLogin(new AdobeConnectAccess(acDomain, acLogin, password), out sessionToken);

                if (acPrincipal == null)
                    return null;

                var applicationUser = new AdobeConnectUser
                {
                    Id = acPrincipal.UserId,
                    UserName = acLogin,

                    CompanyToken = companyToken,
                    AcDomain = acDomain,
                    AcSessionToken = sessionToken,
                };
                //Store.CreateAsync(applicationUser);
                return applicationUser;
                
            });
            return taskInvoke;
        }

        private UserInfo TryLogin(IAdobeConnectAccess credentials, out string sessionToken)
        {
            string apiUrl = credentials.Domain + "/api/xml";
            var connectionDetails = new ConnectionDetails
            {
                ServiceUrl = apiUrl,
            };
            var provider = new AdobeConnectProvider(connectionDetails);

            LoginResult result = provider.Login(new UserCredentials(credentials.Login, credentials.Password));
            if (!result.Success)
            {
                //_logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                sessionToken = null;
                return null;
            }

            sessionToken = result.Status.SessionInfo;

            return _userGroupPermissionProvider.UserHasGroupPermission(provider, result.User) ? result.User : null;
        }
        
    }
}
