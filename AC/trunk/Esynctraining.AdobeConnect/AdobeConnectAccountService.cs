using System;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
//using NodaTime.TimeZones;

namespace Esynctraining.AdobeConnect
{
    public sealed class AdobeConnectAccountService : IAdobeConnectAccountService
    {
        private readonly ILogger _logger;


        public AdobeConnectAccountService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public IAdobeConnectProxy GetProvider(IAdobeConnectAccess credentials, bool login)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));

            var connectionDetails = new ConnectionDetails(credentials.Domain);
            var provider = new AdobeConnectProvider(connectionDetails);
            if (login)
            {
                var userCredentials = new UserCredentials(credentials.Login, credentials.Password);
                LoginResult result = provider.Login(userCredentials);
                if (!result.Success)
                {
                    _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                    throw new InvalidOperationException("Login to Adobe Connect failed. Status = " + result.Status.GetErrorInfo());
                }
            }

            return new AdobeConnectProxy(provider, _logger, credentials.Domain);
        }

        public IAdobeConnectProxy GetProvider2(IAdobeConnectAccess2 credentials)
        {
            var connectionDetails = new ConnectionDetails(credentials.Domain);
            var provider = new AdobeConnectProvider(connectionDetails);
            LoginResult result = provider.LoginWithSessionId(credentials.SessionToken);
            if (!result.Success || (result.User == null))
            {
                _logger.Error("AdobeConnectAccountService.GetProvider. Login failed. Status = " + result.Status.GetErrorInfo());
                throw new InvalidOperationException("Login to Adobe Connect failed. Status = " + result.Status.GetErrorInfo());
            }

            return new AdobeConnectProxy(provider, _logger, credentials.Domain);
        }

        public ACDetailsDTO GetAccountDetails(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

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
                
                var dto = new ACDetailsDTO
                {
                    Version = commonInfo.CommonInfo.Version,
                    PasswordPolicies = ParsePasswordPolicies(fields),
                    Customization = ParseCustomization(fields, provider),
                    TimeZoneJavaId = commonInfo.CommonInfo.TimeZoneJavaId,
                };
                //var timeZone = GetTimeZone(commonInfo.CommonInfo);
                int session;
                int.TryParse(GetField(fields, "web-session-timeout-minutes"), out session);
                dto.SessionTimeout = session;

                //dto.SetTimeZone(timeZone);

                return dto;
            }

            _logger.Error("GetAccountDetails. Account is NULL");
            return null;

        }

        //private TimeZoneInfo GetTimeZone(CommonInfo commonInfo)
        //{
        //    TimeZoneInfo result = null;
        //    if (!string.IsNullOrEmpty(commonInfo.TimeZoneJavaId))
        //    {
        //        result = GetTimeZoneInfoForTzdbId(commonInfo.TimeZoneJavaId);
        //    }

        //    if (result != null)
        //        return result;

        //    var mapping = ACDetailsDTO.TimeZones.First(x => x.Id == commonInfo.TimeZoneId);
        //    result = FindSystemTimeZoneByIdOrDefault(mapping.WindowsTimeZoneId);
        //    if (result != null)
        //        return result;

        //    var timeZoneName = $"Custom time zone: {mapping.BaseUtcOffset}";
        //    result = TimeZoneInfo.CreateCustomTimeZone(
        //        timeZoneName,
        //        TimeSpan.FromMinutes((double)mapping.BaseUtcOffset),
        //        timeZoneName,
        //        timeZoneName);
        //    return result;
        //}

        //private TimeZoneInfo GetTimeZoneInfoForTzdbId(string tzdbId)
        //{
        //    var olsonMappings = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones;
        //    var map = olsonMappings.FirstOrDefault(x =>
        //        x.TzdbIds.Any(z => z.Equals(tzdbId, StringComparison.OrdinalIgnoreCase)));
        //    return map != null ? FindSystemTimeZoneByIdOrDefault(map.WindowsId) : null;
        //}

        //private TimeZoneInfo FindSystemTimeZoneByIdOrDefault(string timezoneId)
        //{
        //    try
        //    {
        //        return TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        //    }
        //    catch (TimeZoneNotFoundException e)
        //    {
        //        _logger.Error($"Timezone not found. Id: {timezoneId}", e);
        //        return null;
        //    }
        //}

        private static CustomizationDTO ParseCustomization(FieldCollectionResult fields, IAdobeConnectProxy provider)
        {
            var logo = new Uri(provider.AdobeConnectRoot, "/webappBanner/custom/images/logos/banner_logo.png");
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
            var loginSameAsEmailField = GetField(fields, "login-same-as-email");
            bool loginSameAsEmail = string.IsNullOrEmpty(loginSameAsEmailField) 
                || "YES".Equals(loginSameAsEmailField, StringComparison.OrdinalIgnoreCase);
            
            return new ACPasswordPoliciesDTO
            {
                PasswordRequiresDigit = passwordRequiresDigit,
                PasswordRequiresCapitalLetter = passwordRequiresCapitalLetter,
                PasswordRequiresSpecialChars = passwordRequiresSpecialChars,
                PasswordMinLength = passwordMinLength,
                PasswordMaxLength = passwordMaxLength,
                LoginSameAsEmail = loginSameAsEmail,
            };
        }

        private static string GetField(FieldCollectionResult value, string fieldName)
        {
            Field field = value.Values.FirstOrDefault(x => x.FieldId == fieldName);
            return field?.Value;
        }
        
    }

}
