using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
using NodaTime.TimeZones;

namespace Esynctraining.AdobeConnect
{
    public sealed class AdobeConnectAccountService : IAdobeConnectAccountService
    {
        private readonly ILogger _logger;

        public AdobeConnectAccountService(ILogger logger)
        {
            _logger = logger;
        }

        public IAdobeConnectProxy GetProvider(IAdobeConnectAccess credentials, bool login)
        {
            string apiUrl = credentials.Domain + "/api/xml";

            var connectionDetails = new ConnectionDetails
            {
                ServiceUrl = apiUrl,
                EventMaxParticipants = 10,
                Proxy =
                new ProxyCredentials
                {
                    Domain = string.Empty,
                    Login = string.Empty,
                    Password = string.Empty,
                    Url = string.Empty,
                },
            };
            string principalId = null;
            var provider = new AdobeConnectProvider(connectionDetails);
            if (login)
            {
                LoginResult result = provider.Login(new UserCredentials(credentials.Login, credentials.Password));
                if (!result.Success)
                {
                    _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                    throw new InvalidOperationException("Login to Adobe Connect failed. Status = " + result.Status.GetErrorInfo());
                }
                principalId = result.User.UserId;
            }

            return new AdobeConnectProxy(provider, _logger, apiUrl);
        }

        public IAdobeConnectProxy GetProvider2(IAdobeConnectAccess2 credentials)
        {
            string apiUrl = credentials.Domain + "/api/xml";

            var connectionDetails = new ConnectionDetails
            {
                ServiceUrl = apiUrl,
                EventMaxParticipants = 10,
            };
            string principalId = null;
            var provider = new AdobeConnectProvider(connectionDetails);
            {
                LoginResult result = provider.LoginWithSessionId(credentials.SessionToken);
                if (result.User == null)
                {
                    _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                    throw new InvalidOperationException("Login to Adobe Connect failed. Status = " + result.Status.GetErrorInfo());
                }
                principalId = result.User.UserId;
            }

            return new AdobeConnectProxy(provider, _logger, apiUrl);
        }

        public ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            CommonInfoResult commonInfo = provider.GetCommonInfo();

            if (!commonInfo.Success)
            {
                _logger.ErrorFormat("GetAccountDetails.GetCommonInfo. AC error:{0}.", commonInfo.Status.GetErrorInfo());
                return null;
            }

            if (commonInfo.CommonInfo.AccountId.HasValue)
            {
                FieldCollectionResult fields = provider.GetAclFields(commonInfo.CommonInfo.AccountId.Value);

                if (fields.Status.Code != StatusCodes.ok)
                {
                    _logger.ErrorFormat("GetAccountDetails.GetAclFields. AC error. Code:{0}.SubCode:{1}.", fields.Status.Code, fields.Status.SubCode);
                    return null;
                }

                //bool loginSameAsEmail = fields.Values.First(x => x.FieldId == "login-same-as-email").Value.Equals("YES", StringComparison.OrdinalIgnoreCase);
                
                return new ACDetailsDTO
                {
                    Version = commonInfo.CommonInfo.Version,
                    TimeZoneInfo = GetTimeZone(commonInfo.CommonInfo),
                    PasswordPolicies = ParsePasswordPolicies(fields),
                    Customization = ParseCustomization(fields, provider),
                };
            }

            _logger.Error("GetAccountDetails. Account is NULL");
            return null;

        }

        private TimeZoneInfo GetTimeZone(CommonInfo commonInfo)
        {
            TimeZoneInfo result = null;
            if (!string.IsNullOrEmpty(commonInfo.TimeZoneJavaId))
            {
                result = GetTimeZoneInfoForTzdbId(commonInfo.TimeZoneJavaId);
            }

            if (result == null)
            {
                var mapping = ACDetailsDTO.TimeZones.First(x => x.Id == commonInfo.TimeZoneId);
                result = FindSystemTimeZoneByIdOrDefault(mapping.WindowsTimeZoneId);
                if (result == null)
                {
                    var timeZoneName = $"Custom time zone: {mapping.BaseUtcOffset}";
                    result = TimeZoneInfo.CreateCustomTimeZone(
                        timeZoneName,
                        TimeSpan.FromMinutes((double)mapping.BaseUtcOffset),
                        timeZoneName,
                        timeZoneName);
                }
            }

            return result;
        }

        private TimeZoneInfo GetTimeZoneInfoForTzdbId(string tzdbId)
        {
            var olsonMappings = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones;
            var map = olsonMappings.FirstOrDefault(x =>
                x.TzdbIds.Any(z => z.Equals(tzdbId, StringComparison.OrdinalIgnoreCase)));
            return map != null ? FindSystemTimeZoneByIdOrDefault(map.WindowsId) : null;
        }

        private TimeZoneInfo FindSystemTimeZoneByIdOrDefault(string timezoneId)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }
            catch (TimeZoneNotFoundException e)
            {
                _logger.Error($"Timezone not found. Id: {timezoneId}", e);
                return null;
            }
        }

        private static CustomizationDTO ParseCustomization(FieldCollectionResult fields, IAdobeConnectProxy provider)
        {
            string domain = provider.ApiUrl.Replace("/api/xml", string.Empty);
            var root = new Uri(domain);
            var logo = new Uri(root, "webappBanner/custom/images/logos/banner_logo.png");
            return new CustomizationDTO
            {
                AccountBannerColor = GetField(fields, "account-banner-color") ?? "FFFFFF",
                BannerTopLinkColor = GetField(fields, "banner-top-link-color") ?? "666666",
                BannerNavTextColor = GetField(fields, "banner-nav-text-color") ?? "666666",
                BannerNavSelColor = GetField(fields, "banner-nav-sel-color") ?? "E9E9E9",
                AccountHeaderColor = GetField(fields, "account-header-color") ?? "A7ACB1",
                BannerLogoUrl = logo.ToString(),
            };
        }

        private static ACPasswordPoliciesDTO ParsePasswordPolicies(FieldCollectionResult fields)
        {
            bool passwordRequiresDigit = "YES".Equals(GetField(fields, "password-requires-digit"), StringComparison.OrdinalIgnoreCase);
            bool passwordRequiresCapitalLetter = "YES".Equals(GetField(fields, "password-requires-capital-letter"), StringComparison.OrdinalIgnoreCase);
            string passwordRequiresSpecialChars = GetField(fields, "password-requires-special-chars");

            int passwordMinLength = int.Parse(GetField(fields, "password-min-length") ?? "4");
            int passwordMaxLength = int.Parse(GetField(fields, "password-max-length") ?? "32");

            return new ACPasswordPoliciesDTO
            {
                PasswordRequiresDigit = passwordRequiresDigit,
                PasswordRequiresCapitalLetter = passwordRequiresCapitalLetter,
                PasswordRequiresSpecialChars = passwordRequiresSpecialChars,
                PasswordMinLength = passwordMinLength,
                PasswordMaxLength = passwordMaxLength,
            };
        }

        private static string GetField(FieldCollectionResult value, string fieldName)
        {
            Field field = value.Values.FirstOrDefault(x => x.FieldId == fieldName);
            if (field == null)
            {
                return null;
            }

            return field.Value;
        }
        
    }

}
