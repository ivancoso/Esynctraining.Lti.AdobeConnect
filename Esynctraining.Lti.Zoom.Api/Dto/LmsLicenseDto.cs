using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class LmsLicenseDto
    {
        public int Id { get; set; }
        public Guid ConsumerKey { get; set; }
        public Guid SharedSecret { get; set; }

        public int ProductId { get; set; }

        public string Domain { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public T GetSetting<T>(string settingName)
        {
            return (Settings.ContainsKey(settingName))
                ? (T) Convert.ChangeType(Settings[settingName], typeof(T)) // assuming that we convert to primitive type
                : default(T);
        }

        public T GetSetting<T>(string settingName, T defaultValue)
        {
            return (Settings.ContainsKey(settingName))
                ? (T) Convert.ChangeType(Settings[settingName], typeof(T)) // assuming that we convert to primitive type
                : defaultValue;
        }

        public Dictionary<string, object> GetLMSSettings(LmsUserSession session)
        {
            Dictionary<string, object> result = null;
            switch (ProductId)
            {
                case 1010:
                    var optionNamesForCanvas = new List<string>
                    {
                        LmsLicenseSettingNames.CanvasOAuthId,
                        LmsLicenseSettingNames.CanvasOAuthKey
                    };
                    result = Settings.Where(x => optionNamesForCanvas.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object) v.Value);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, Domain);
                    result.Add(LmsUserSettingNames.Token, session.Token);
                    break;

                case 1020:
                    var optionNamesForBuzz = new List<string>
                    {
                        LmsLicenseSettingNames.BuzzAdminUsername,
                        LmsLicenseSettingNames.BuzzAdminPassword
                    };
                    result = Settings.Where(x => optionNamesForBuzz.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object) v.Value);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, Domain);
                    break;
                case 1030:
                    var optionNamesForSchoology = new List<string>
                    {
                        LmsLicenseSettingNames.SchoologyConsumerKey,
                        LmsLicenseSettingNames.SchoologyConsumerSecret
                    };
                    result = Settings.Where(x => optionNamesForSchoology.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object)v.Value);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, Domain);
                    break;
                case 1040:
                    var optionNamesForBlackBoard = new List<string>
                    {
                        LmsLicenseSettingNames.AdminUsername,
                        LmsLicenseSettingNames.AdminPassword,
                        //LmsLicenseSettingNames.BlackBoardEnableProxyToolMode,
                        LmsLicenseSettingNames.BlackBoardProxyToolPassword,
                    };
                    result = Settings.Where(x => optionNamesForBlackBoard.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object)v.Value);

                    result.Add(LmsLicenseSettingNames.BlackBoardEnableProxyToolMode, Settings[LmsLicenseSettingNames.BlackBoardEnableProxyToolMode] == "True");

                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, Domain);

                    result.Add(LmsLicenseSettingNames.BlackBoardUseSSL, true);
                    break;

                default:
                    throw new NotImplementedException($"ProductId {ProductId} is not implemented.");
            }

            return result;
        }
    }
}
