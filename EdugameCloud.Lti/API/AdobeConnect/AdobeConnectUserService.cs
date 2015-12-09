using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Domain.Entities;
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
            bool searchByEmail = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;
            Principal principal = this.GetPrincipalByLoginOrEmail(provider, login, email, searchByEmail);

            if (principal == null && !denyUserCreation)
            {
                if (searchByEmail && string.IsNullOrWhiteSpace(email))
                    throw new WarningMessageException(Resources.Messages.CantCreatePrincipalWithEmptyEmail);

                principal = CreatePrincipal(provider, login, email, firstName, lastName, searchByEmail);
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
            bool searchByEmail = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;

            Principal principal = null;
            if (principalCache != null)
            {
                principal = GetPrincipalByLoginOrEmail(principalCache, login, email, searchByEmail);
            }

            if (principal == null)
            {
                principal = GetPrincipalByLoginOrEmail(provider, login, email, searchByEmail);
            }

            if (!denyUserCreation && (principal == null))
            {
                if (searchByEmail && string.IsNullOrWhiteSpace(email))
                    throw new WarningMessageException(Resources.Messages.CantCreatePrincipalWithEmptyEmail);

                principal = CreatePrincipal(provider, login, email, firstName, lastName, searchByEmail);
            }

            return principal;
        }

        public Principal GetPrincipalByLoginOrEmail(
            IAdobeConnectProxy provider,
            string login,
            string email,
            bool searchByEmail)
        {
            if (searchByEmail && string.IsNullOrWhiteSpace(email))
                return null;
            if (!searchByEmail && string.IsNullOrWhiteSpace(login))
                return null;

            PrincipalCollectionResult result = searchByEmail 
                ? provider.GetAllByEmail(email) 
                : provider.GetAllByLogin(login);
            if (!result.Success)
                return null;
            
            return result.Return(x => x.Values, Enumerable.Empty<Principal>()).FirstOrDefault();
        }

        private Principal GetPrincipalByLoginOrEmail(
            IEnumerable<Principal> principalCache,
            string login,
            string email,
            bool searchByEmail)
        {
            if (searchByEmail && string.IsNullOrWhiteSpace(email))
                return null;
            if (!searchByEmail && string.IsNullOrWhiteSpace(login))
                return null;

            return searchByEmail
                ? principalCache.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                : principalCache.FirstOrDefault(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
        }

        private Principal CreatePrincipal(
            IAdobeConnectProxy provider,
            string login,
            string email,
            string firstName,
            string lastName,
            bool acUsesEmailAsLogin)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new WarningMessageException("Adobe Connect User's First Name can't be empty.");
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new WarningMessageException("Adobe Connect User's Last Name can't be empty.");
            }

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