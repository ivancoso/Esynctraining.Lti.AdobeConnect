using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class AdobeConnectUserService : IAdobeConnectUserService
    {
        public Principal GetOrCreatePrincipal(
            AdobeConnectProvider provider, 
            string login, 
            string email, 
            string firstName, 
            string lastName, 
            LmsCompany lmsCompany)
        {
            bool searchByEmailFirst = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;
            Principal principal = this.GetPrincipalByLoginOrEmail(provider, login, email, searchByEmailFirst);

            if (principal == null && !denyUserCreation)
            {
                var setup = new PrincipalSetup
                {
                    Email = string.IsNullOrWhiteSpace(email) ? null : email, 
                    FirstName = firstName, 
                    LastName = lastName, 
                    Name = login, 
                    Login = login, 
                    Type = PrincipalTypes.user, 
                };

                PrincipalResult pu = provider.PrincipalUpdate(setup);

                if (!pu.Success)
                {
                    string additionalData = string.Format("firstName: {0}, lastName: {1}, login: {2}, email: {3}", firstName, lastName, login, email);
                    throw new InvalidOperationException(string.Format("AC.PrincipalUpdate error. {0}. Additional Data: {1}", pu.Status.GetErrorInfo(), additionalData));
                }

                if (pu.Principal != null)
                {
                    principal = pu.Principal;
                }
            }

            return principal;
        }

        public Principal GetOrCreatePrincipal2(
            AdobeConnectProvider provider,
            string login,
            string email,
            string firstName,
            string lastName,
            LmsCompany lmsCompany,
            IEnumerable<Principal> principalCache)
        {
            bool searchByEmailFirst = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;

            Principal principal = null;
            if (principalCache != null)
            {
                principal = GetPrincipalByLoginOrEmail(principalCache, login, email, searchByEmailFirst);
            }

            if (principal == null)
            {
                principal = GetPrincipalByLoginOrEmail(provider, login, email, searchByEmailFirst);
            }

            if (!denyUserCreation && (principal == null))
            {
                var setup = new PrincipalSetup
                {
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    FirstName = firstName,
                    LastName = lastName,
                    Name = login,
                    Login = login,
                    Type = PrincipalTypes.user,
                };

                PrincipalResult pu = provider.PrincipalUpdate(setup);

                // TODO: review and add
                // if (!pu.Success)
                // {
                // throw new InvalidOperationException("AC.PrincipalUpdate error", pu.Status.UnderlyingExceptionInfo);
                // }
                if (pu.Principal != null)
                {
                    principal = pu.Principal;
                }
            }



            return principal;
        }

        public Principal GetPrincipalByLoginOrEmail(
            AdobeConnectProvider provider,
            string login,
            string email,
            bool searchByEmailFirst)
        {
            Principal principal = null;

            if (searchByEmailFirst && !string.IsNullOrWhiteSpace(email))
            {
                PrincipalCollectionResult resultByEmail = provider.GetAllByEmail(email);
                if (!resultByEmail.Success)
                {
                    return null;
                }

                principal = resultByEmail.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
            }

            if (principal == null && !string.IsNullOrWhiteSpace(login))
            {
                PrincipalCollectionResult resultByLogin = provider.GetAllByLogin(login);
                if (!resultByLogin.Success)
                {
                    return null;
                }

                principal = resultByLogin.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
            }

            if (!searchByEmailFirst && principal == null && !string.IsNullOrWhiteSpace(email))
            {
                PrincipalCollectionResult resultByEmail = provider.GetAllByEmail(email);
                if (!resultByEmail.Success)
                {
                    return null;
                }

                principal = resultByEmail.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
            }

            return principal;
        }

        private Principal GetPrincipalByLoginOrEmail(
            IEnumerable<Principal> principalCache, 
            string login, 
            string email, 
            bool searchByEmailFirst)
        {
            Principal principal = null;

            if (searchByEmailFirst && !string.IsNullOrWhiteSpace(email))
            {
                principal = principalCache.FirstOrDefault(
                    p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            if ((principal == null) && !string.IsNullOrWhiteSpace(login))
            {
                principal = principalCache.FirstOrDefault(
                    p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            }

            if (!searchByEmailFirst && (principal == null) && !string.IsNullOrWhiteSpace(email))
            {
                principal = principalCache.FirstOrDefault(
                    p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }

            return principal;
        }
    }
}