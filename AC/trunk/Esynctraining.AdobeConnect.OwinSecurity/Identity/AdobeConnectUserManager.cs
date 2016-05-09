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
        public AdobeConnectUserManager() : base(new EdugameCloudUserStore<AdobeConnectUser>())
        {
            //We can retrieve Old System Hash Password and can encypt or decrypt old password using custom approach. 
            //When we want to reuse old system password as it would be difficult for all users to initiate pwd change as per Idnetity Core hashing. 
            //this.PasswordHasher = new EdugameCloudPasswordHasher();
            
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

            // TRICK: meeting-hosts and administrators can access
            var meetingHostsGroup = provider.GetGroupsByType("live-admins");
            if (meetingHostsGroup.Item1.Code != StatusCodes.ok)
                return null;

            var meetingHosts = provider.GetGroupPrincipalUsers(meetingHostsGroup.Item2.First().PrincipalId, result.User.UserId);
            bool isMeetingHost = meetingHosts.Success && meetingHosts.Values.Any(x => x.Login == result.User.Login);
            if (isMeetingHost)
                return result.User;

            var adminHostsGroup = provider.GetGroupsByType("admins");
            if (adminHostsGroup.Item1.Code != StatusCodes.ok)
                return null;

            var admins = provider.GetGroupPrincipalUsers(adminHostsGroup.Item2.First().PrincipalId, result.User.UserId);
            bool isAdmin = admins.Success && admins.Values.Any(x => x.Login == result.User.Login);
            if (isAdmin)
                return result.User;

            return null;
        }

    }

}
