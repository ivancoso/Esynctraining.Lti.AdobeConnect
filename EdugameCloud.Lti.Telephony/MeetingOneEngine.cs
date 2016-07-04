using System;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Telephony.Configuration;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.MeetingOne;
using Esynctraining.MeetingOne.RestClient;

namespace EdugameCloud.Lti.Telephony
{
    public sealed class MeetingOneEngine : ITelephonyProfileEngine
    {
        private static readonly string ClassName = "com.meetingone.adobeconnect.MeetingOneAdobeConnectAdaptor";
        private readonly ILogger _logger;
        private readonly string _baseAddress;


        public MeetingOneEngine(ILogger logger, string baseAddress = null)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _baseAddress = baseAddress ?? MeetingOneConfigurationSection.Current.ApiUrl;
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
                var meetingOne = profiles.Values.FirstOrDefault(x => x.ClassName == ClassName);
                string providerId = meetingOne.ProviderId;

                if (meetingOne.ProviderStatus != "enabled")
                    throw new InvalidOperationException("MeetingOne provider is not enabled");

                var access = new AccessDetails
                {
                    UserName = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.MeetingOne.UserName),
                    SecretHashKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.MeetingOne.SecretHashKey),
                    OwningAccountNumber = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Telephony.MeetingOne.OwningAccountNumber),
                };

                var room = new RoomDto
                {
                    Host = new Host
                    {
                        Email = param.lis_person_contact_email_primary,
                        FirstName = param.lis_person_name_given,
                        LastName = param.lis_person_name_family,
                    },
                };

                // TODO: DI
                RoomDto result = null;
                try
                {
                    var client = new MeetingOneClient(_baseAddress, _logger);
                    result = await client.CreateRoomAsync(access, room).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error("MeetingOneClient.CreateRoomAsync failed.", ex);
                    return null;
                }

                var acProfile = new TelephonyProfileUpdateItem
                {
                    ProfileName = profileName,
                    // ProfileStatus = "enabled",
                    ProviderId = providerId,

                    ProviderFields = new MeetingOneProviderFields
                    {
                        ConferenceId = result.Number,
                        HostPin = result.HostPIN,
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
