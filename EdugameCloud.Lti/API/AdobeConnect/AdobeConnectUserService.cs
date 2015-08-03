using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core;
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
            IAdobeConnectProxy provider, 
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
                principal = CreatePrincipal(provider, login, email, firstName, lastName, searchByEmailFirst);
            }

            return principal;
        }

        public Principal GetOrCreatePrincipal2(
            IAdobeConnectProxy provider,
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
                principal = CreatePrincipal(provider, login, email, firstName, lastName, searchByEmailFirst);
            }

            return principal;
        }

        public Principal GetPrincipalByLoginOrEmail(
            IAdobeConnectProxy provider,
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

        private Principal CreatePrincipal(
            IAdobeConnectProxy provider,
            string login,
            string email,
            string firstName,
            string lastName,
            bool acUsesEmailAsLogin)
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

            PrincipalResult pu = provider.PrincipalUpdate(setup, false, false);

            if (!pu.Success)
            {
                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.duplicate)
                {
                    if (acUsesEmailAsLogin)
                    {
                        UserCollectionResult guestsByEmail = provider.ReportGuestsByEmail(email);
                        if (guestsByEmail.Success && guestsByEmail.Values.Any())
                            throw new WarningMessageException(string.Format("Unable to sync users. Guest record with email '{0}' already exists in Adobe Connect. Please contact AC administrator to promote guest to user.", email));
                    }
                    else
                    {
                        UserCollectionResult guestsByLogin = provider.ReportGuestsByLogin(login);
                        if (guestsByLogin.Success && guestsByLogin.Values.Any())
                            throw new WarningMessageException(string.Format("Unable to sync users. Guest record with login '{0}' already exists in Adobe Connect. Please contact AC administrator to promote guest to user.", login));
                    }
                }
            }

            if (pu.Principal != null)
            {
                return pu.Principal;
            }
            return null;
        }

    }

}