﻿using System;
using System.Linq;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.MeetingOne;
using Esynctraining.MeetingOne.RestClient;

namespace EdugameCloud.Lti.Telephony
{
    public sealed class MeetingOneEngine : ITelephonyProfileEngine
    {
        private static readonly string ClassName = "com.meetingone.adobeconnect.MeetingOneAdobeConnectAdaptor";
        private readonly ILogger _logger;
        private readonly string _baseAddress;


        public MeetingOneEngine(ILogger logger, MeetingOneSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _baseAddress = settings.ApiUrl;
        }
        

        public TelephonyProfile CreateProfile(ILmsLicense lmsCompany, ILtiParam param, string profileName, IAdobeConnectProxy acProxy)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(profileName))
                throw new ArgumentException("Non-empty value expected", nameof(profileName));
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
                    UserName = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.UserName),
                    SecretHashKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.SecretHashKey),
                    OwningAccountNumber = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.OwningAccountNumber),
                };

                var room = new RoomDto
                {
                    Host = new Host
                    {
                        Email = param.lis_person_contact_email_primary,
                        FirstName = param.PersonNameGiven,
                        LastName = param.PersonNameFamily,
                        MailingAddress = param.lis_person_contact_email_primary,
                    },

                    RoomSettings = new RoomSettings
                    {
                        DialoutEnabled = true,
                        AllowDomesticDialouts = true,
                        AllowInternationalDialouts = true,
                    },
                };

                // TODO: DI
                RoomDto result = null;
                try
                {
                    var client = new MeetingOneClient(_baseAddress, _logger);
                    result = client.CreateRoom(access, room);
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

        public void DeleteProfile(ILmsLicense lmsCompany, TelephonyProfileInfoResult profile)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            string roomNumber = profile.TelephonyProfileFields.First(x => x.Key == "x-tel-meetingone-conference-id").Value;
            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new InvalidOperationException("Can't find meeting-one room number");

            var access = new AccessDetails
            {
                UserName = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.UserName),
                SecretHashKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.SecretHashKey),
                OwningAccountNumber = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.OwningAccountNumber),
            };

            bool deleted = false;
            try
            {
                var client = new MeetingOneClient(_baseAddress, _logger);
                deleted = client.DeleteRoom(access, roomNumber);
            }
            catch (Exception ex)
            {
                _logger.Error($"MeetingOneClient.DeleteRoom failed. RoomNumber:'{roomNumber}'. LmsCompanyId:{lmsCompany.Id}", ex);
            }

            if (!deleted)
                throw new InvalidOperationException("MeetingONE API hasn't deleted room.");
        }

    }

}
