using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Common.Dto
{
    public class LmsLicenseDto
    {
        public int Id { get; set; }
        public Guid ConsumerKey { get; set; }
        public Guid SharedSecret { get; set; }

        public int ProductId { get; set; }

        public string Domain { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public ZoomUserDto ZoomUserDto { get; set; }

        public LanguageDto Language { get; set; }

        public ICollection<LmsLicenseRoleMappingDto> LmsLicenseRoleMappings { get; set; }

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
            switch ((LMS)ProductId)
            {
                case LMS.Canvas:
                    var optionNamesForCanvas = new List<string>
                    {
                        LmsLicenseSettingNames.CanvasOAuthId,
                        LmsLicenseSettingNames.CanvasOAuthKey,
                        LmsLicenseSettingNames.UseGeneratedToken,
                        LmsLicenseSettingNames.CanvasGeneratedToken
                    };
                    result = Settings.Where(x => optionNamesForCanvas.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object) v.Value);

                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, RemoveHttpProtocolAndTrailingSlash(Domain));

                    //NOTE!
                    //Mike wants to use https://canvas.instructure.com/. It is free canvas instance for teacheres.
                    //We cannot get Admin user from this instanse of canvas. Admin user is needed to get OauthId, OauthKey.
                    //So we will use generated token. One techer hasto have only one token. For each techer we have to create a new license.
                    if (result.ContainsKey(LmsLicenseSettingNames.UseGeneratedToken) && Boolean.Parse(result[LmsLicenseSettingNames.UseGeneratedToken].ToString()))
                    {
                        result.Add(LmsUserSettingNames.Token, (string)result[LmsLicenseSettingNames.CanvasGeneratedToken]);
                    }
                    else
                    {
                        result.Add(LmsUserSettingNames.Token, session.Token);
                    }
                    
                    result.Add(LmsUserSettingNames.RefreshToken, session.RefreshToken);
                    result.Add(LmsLicenseSettingNames.EnableCanvasExportToCalendar, Settings.ContainsKey(LmsLicenseSettingNames.EnableCanvasExportToCalendar) && Settings[LmsLicenseSettingNames.EnableCanvasExportToCalendar] == "True");
                    
                    break;

                case LMS.AgilixBuzz:
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
                case LMS.Schoology:
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
                case LMS.BlackBoard:
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
                case LMS.Moodle:
                    var optionsNameForMoodle = new List<string>
                    {
                        LmsLicenseSettingNames.MoodleAdminUserName,
                        LmsLicenseSettingNames.MoodleAdminUserPassword,
                        LmsLicenseSettingNames.MoodleCoreServiceToken,
                        LmsLicenseSettingNames.MoodleEgcServiceToken,
                        LmsLicenseSettingNames.MoodleAuthMode
                    };
                    result = Settings.Where(x => optionsNameForMoodle.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object)v.Value);
                    
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.UseSSL, Domain.StartsWith("https"));
                    result.Add(LmsLicenseSettingNames.LmsDomain, RemoveHttpProtocolAndTrailingSlash(Domain));

                    break;
                case LMS.Sakai:
                    result = new Dictionary<string, object>();
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey.ToString());
                    result.Add(LmsLicenseSettingNames.LicenseSecret, SharedSecret.ToString());
                    //TODO Fix LanguageID
                    result.Add(LmsLicenseSettingNames.LanguageId, 5);
                    result.Add(LmsLicenseSettingNames.UseSSL, Domain.StartsWith("https"));
                    result.Add(LmsLicenseSettingNames.LmsDomain, Domain);
                    break;
                case LMS.Desire2Learn:
                    var optionsNameForBrightSpace = new List<string>
                    {
                        LmsLicenseSettingNames.BrigthSpaceAppId,
                        LmsLicenseSettingNames.BrigthSpaceAppKey,
                        LmsLicenseSettingNames.BrigthSpaceAdminName,
                        LmsLicenseSettingNames.BrigthSpaceAdminPassword,
                    };
                    result = Settings.Where(x => optionsNameForBrightSpace.Any(o => o == x.Key))
                        .ToDictionary(k => k.Key, v => (object)v.Value);

                    result.Add(LmsUserSettingNames.Token, session.Token);
                    result.Add(LmsLicenseSettingNames.BrightspaceApiVersion, "1.4");
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, Domain);
                    result.Add(LmsLicenseSettingNames.BrightSpaceAllowAdminAdditionToCourse, false);
                    break;
                default:
                    throw new NotImplementedException($"ProductId {ProductId} is not implemented.");
            }

            return result;
        }

        private static string RemoveHttpProtocolAndTrailingSlash(string url)
        {
            if (url != null)
            {
                if (url.StartsWith(HttpScheme.Http, StringComparison.OrdinalIgnoreCase))
                {
                    url = url.Substring(HttpScheme.Http.Length);
                }

                if (url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase))
                {
                    url = url.Substring(HttpScheme.Https.Length);
                }

                while (url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
            }

            return url;
        }
    }
}
