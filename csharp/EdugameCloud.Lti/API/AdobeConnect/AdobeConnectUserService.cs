using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Resources;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
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
            ILmsLicense lmsCompany)
        {
            bool searchByEmail = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault();
            bool denyUserCreation = lmsCompany.DenyACUserCreation;
            Principal principal = this.GetPrincipalByLoginOrEmail(provider, login, email, searchByEmail);

            if (principal == null && !denyUserCreation)
            {
                if (searchByEmail && string.IsNullOrWhiteSpace(email))
                    throw new Core.WarningMessageException(Resources.Messages.CantCreatePrincipalWithEmptyEmail);

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
            ILmsLicense lmsCompany,
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
                    throw new Core.WarningMessageException(Resources.Messages.CantCreatePrincipalWithEmptyEmail);

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
                throw new Core.WarningMessageException("Adobe Connect User's First Name can't be empty.");
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new Core.WarningMessageException("Adobe Connect User's Last Name can't be empty.");
            }

            var setup = new PrincipalSetup
            {
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                FirstName = firstName,
                LastName = lastName,
                Name = login,
                Login = login,
                Type = PrincipalType.user,
            };

            PrincipalResult pu = provider.PrincipalUpdate(setup, false, false);

            if (!pu.Success)
            {
                if (pu.Status.InvalidField == "login" && pu.Status.SubCode == StatusSubCodes.duplicate)
                {
                    if (acUsesEmailAsLogin)
                    {
                        UserCollectionResult guestsByEmail = provider.ReportGuestsByEmail(HttpUtilsInternal.UrlEncode(email));
                        if (guestsByEmail.Success && guestsByEmail.Values.Any())
                            throw new Core.WarningMessageException(string.Format(Messages.PrincipalEmailUsedForGuest, email));
                    }
                    else
                    {
                        UserCollectionResult guestsByLogin = provider.ReportGuestsByLogin(login);
                        if (guestsByLogin.Success && guestsByLogin.Values.Any())
                            throw new Core.WarningMessageException(string.Format(Messages.PrincipalLoginUsedForGuest, login));
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

    internal class HttpUtilsInternal
    {
        /// <summary>
        /// The url encode.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UrlEncode(string text)
        {
            return UrlEncode(text, Encoding.UTF8);
        }

        private static string UrlEncode(string text, Encoding encoding)
        {
            if (text == null)
            {
                return null;
            }

            if (text == string.Empty)
            {
                return string.Empty;
            }

            var bytes = encoding.GetBytes(text);

            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, 0, bytes.Length));
        }

        /// <summary>
        /// The url encode to bytes.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The bytes.
        /// </returns>
        public static byte[] UrlEncodeToBytes(string text)
        {
            return UrlEncodeToBytes(text, Encoding.UTF8);
        }

        private static byte[] UrlEncodeToBytes(string text, Encoding encoding)
        {
            if (text == null)
            {
                return null;
            }

            if (text == string.Empty)
            {
                return new byte[0];
            }

            var bytes = encoding.GetBytes(text);

            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        private static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return null;
            }

            var len = bytes.Length;

            if (len == 0)
            {
                return new byte[0];
            }

            if (offset < 0 || offset >= len)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0 || count > len - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var result = new MemoryStream(count);
            var end = offset + count;

            for (var i = offset; i < end; i++)
            {
                UrlEncodeChar((char)bytes[i], result, false);
            }

            return result.ToArray();
        }

        private static void UrlEncodeChar(char symbol, Stream result, bool isUnicode)
        {
            var hexChars = "0123456789abcdef".ToCharArray();
            const string NotEncoded = "!'()*-._";

            if (symbol > 255)
            {
                var i = (int)symbol;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');

                var idx = i >> 12;

                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);

                return;
            }

            if (symbol > ' ' && NotEncoded.IndexOf(symbol) != -1)
            {
                result.WriteByte((byte)symbol);
                return;
            }

            if (symbol == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }

            if ((symbol < '0') || (symbol < 'A' && symbol > '9') || (symbol > 'Z' && symbol < 'a') || (symbol > 'z'))
            {
                if (isUnicode && symbol > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                {
                    result.WriteByte((byte)'%');
                }

                var idx = symbol >> 4;

                result.WriteByte((byte)hexChars[idx]);
                idx = symbol & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
            {
                result.WriteByte((byte)symbol);
            }
        }

    }

}