﻿using System;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Arkadin.ServicesClient;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Telephony
{
    public sealed class ArkadinEngine : ITelephonyProfileEngine
    {
        private static readonly string ClassName = "com.macromedia.breeze_ext.arkadin.ArkadinAdaptor";
        private readonly ILogger _logger;


        //http://ap-wapi.arkadin.com/WAPIFullService.asmx
        public ArkadinEngine(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _logger = logger;
        }


        public async Task<TelephonyProfile> CreateProfileAsync(LmsCompany lmsCompany, LtiParamDTO param, string profileName, IAdobeConnectProxy acProxy)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(profileName))
                throw new ArgumentNullException("Non-empty value expected", nameof(profileName));
            if (acProxy == null)
                throw new ArgumentNullException(nameof(acProxy));

            try
            {
                var profiles = acProxy.TelephonyProviderList(null);
                var arkadin = profiles.Values.FirstOrDefault(x => x.ClassName == ClassName);
                string providerId = arkadin.ProviderId;

                if (arkadin.ProviderStatus != "enabled")
                    throw new InvalidOperationException("Arkadin provider is not enabled");

                var access = new AccessDetails
                {
                    Login = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.Arkadin.UserName),
                    //Password = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.Arkadin.Password),
                    //BridgeId = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.Arkadin.BridgeId),
                };

                // TODO: DI
                var client = new ArkadinClient();
                var result = await client.CreateProfile(access);

                var acProfile = new TelephonyProfileUpdateItem
                {
                    ProfileName = profileName,
                    // ProfileStatus = "enabled",
                    ProviderId = providerId,

                    ProviderFields = new ArkadinProviderFields
                    {
                        // TODO:
                        //ConferenceId
                    },
                };

                TelephonyProfileInfoResult createdProfile = acProxy.TelephonyProfileUpdate(acProfile, false);
                return createdProfile.TelephonyProfile;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateProfileAsync error. CompanyLicenseId: {lmsCompany.Id}.", ex);
                throw;
            }
        }

    }

}
