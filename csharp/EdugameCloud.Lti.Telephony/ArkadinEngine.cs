//using System;
//using System.Linq;
//using EdugameCloud.Lti.Core.Constants;
//using EdugameCloud.Lti.Domain.Entities;
//using Esynctraining.AC.Provider.DataObjects.Results;
//using Esynctraining.AC.Provider.Entities;
//using Esynctraining.AdobeConnect;
//using Esynctraining.Core.Logging;

//namespace EdugameCloud.Lti.Telephony
//{
//    public sealed class ArkadinEngine //: ITelephonyProfileEngine
//    {
//        private static readonly string ClassName = "com.macromedia.breeze_ext.arkadin.ArkadinAdaptor";
//        private readonly ILogger _logger;


//        //http://ap-wapi.arkadin.com/WAPIFullService.asmx
//        public ArkadinEngine(ILogger logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }


////        public TelephonyProfile CreateProfile(ILmsLicense lmsCompany, LtiParamDTO param, string profileName, IAdobeConnectProxy acProxy)
////        {
////            if (lmsCompany == null)
////                throw new ArgumentNullException(nameof(lmsCompany));
////            if (param == null)
////                throw new ArgumentNullException(nameof(param));
////            if (string.IsNullOrWhiteSpace(profileName))
////                throw new ArgumentException("Non-empty value expected", nameof(profileName));
////            if (acProxy == null)
////                throw new ArgumentNullException(nameof(acProxy));

////            throw new NotImplementedException();

////            try
////            {
////                var profiles = acProxy.TelephonyProviderList(null);
////                var arkadin = profiles.Values.FirstOrDefault(x => x.ClassName == ClassName);
////                string providerId = arkadin.ProviderId;

////                if (arkadin.ProviderStatus != "enabled")
////                    throw new InvalidOperationException("Arkadin provider is not enabled");

////                var access = new AccessDetails
////                {
////                    Login = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.UserName),
////                    Password = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.Password),
////                    BridgeId = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.BridgeId),
////                };


////                // TODO: DI
////                RoomDto result = null;
////                try
////                {
////                    var client = new ArkadinClient(_baseAddress, _logger);
////                    result = client.CreateRoom(access, room);
////                }
////                catch (Exception ex)
////                {
////                    _logger.Error("ArkadinClient.CreateRoomAsync failed.", ex);
////                    return null;
////                }

////                var acProfile = new TelephonyProfileUpdateItem
////                {
////                    ProfileName = profileName,
////                    // ProfileStatus = "enabled",
////                    ProviderId = providerId,

////                    ProviderFields = new ArkadinProviderFields
////                    {
////                        ConferenceId = result.Number,
////                        ModeratorCode = result.mo
////                    },
////                };

////                TelephonyProfileInfoResult createdProfile = acProxy.TelephonyProfileUpdate(acProfile, false);
////                return createdProfile.TelephonyProfile;
////            }
////            catch (Exception ex)
////            {
////                _logger.Error($"CreateProfileAsync error. CompanyLicenseId: {lmsCompany.Id}.", ex);
////                throw;
////            }
////}

//        public TelephonyProfile CreateProfile(string userName, string userEmail, IAdobeConnectProxy acProxy)
//        {
//            if (acProxy == null)
//                throw new ArgumentNullException(nameof(acProxy));
            
//            try
//            {
//                var profiles = acProxy.TelephonyProviderList(null);
//                var arkadin = profiles.Values.FirstOrDefault(x => x.ClassName == ClassName);
//                string providerId = arkadin.ProviderId;

//                if (arkadin.ProviderStatus != "enabled")
//                    throw new InvalidOperationException("Arkadin provider is not enabled");

//                var access = new AccessDetails
//                {
//                    Login = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.UserName),
//                    Password = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.Password),
//                    BridgeId = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.BridgeId),
//                };


//                // TODO: DI
//                RoomDto result = null;
//                try
//                {
//                    var client = new ArkadinClient(_baseAddress, _logger);
//                    result = client.CreateRoom(access, room);
//                }
//                catch (Exception ex)
//                {
//                    _logger.Error("MeetingOneClient.CreateRoomAsync failed.", ex);
//                    return null;
//                }

//                var acProfile = new TelephonyProfileUpdateItem
//                {
//                    ProfileName = profileName,
//                    // ProfileStatus = "enabled",
//                    ProviderId = providerId,

//                    ProviderFields = new MeetingOneProviderFields
//                    {
//                        ConferenceId = result.Number,
//                        HostPin = result.HostPIN,
//                    },
//                };

//                TelephonyProfileInfoResult createdProfile = acProxy.TelephonyProfileUpdate(acProfile, false);
//                return createdProfile.TelephonyProfile;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error($"CreateProfileAsync error. CompanyLicenseId: {lmsCompany.Id}.", ex);
//                throw;
//            }
//        }

//        public void DeleteProfile(ILmsLicense lmsCompany, TelephonyProfileInfoResult profile)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}
