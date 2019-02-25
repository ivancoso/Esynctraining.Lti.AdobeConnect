using System.Linq;
using Esynctraining.Lti.Lms.Common.Constants;

namespace EdugameCloud.Lti.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Newtonsoft.Json;

    /// <summary>
    /// The company LMS.
    /// </summary>
    public class LmsCompany : Entity, ILmsLicense
    {
        public static readonly int SpanishLanguageId = 10;


        /// <summary>
        /// The LMS domain.
        /// </summary>
        private string lmsDomain;

        #region Public Properties

        /// <summary>
        /// Folder in AC where meetings are stored.
        /// </summary>
        public virtual string ACScoId { get; set; }

        /// <summary>
        /// sco-id of shared-meeting-templates folder (not longer used)
        /// </summary>
        [Obsolete]
        public virtual string ACTemplateScoId { get; set; }

        /// <summary>
        /// Gets or sets the AC password.
        /// </summary>
        public virtual string AcPassword { get; set; }

        /// <summary>
        /// Gets or sets the AC server.
        /// </summary>
        public virtual string AcServer { get; set; }

        /// <summary>
        /// Gets or sets the AC username.
        /// </summary>
        public virtual string AcUsername { get; set; }

        /// <summary>
        /// Gets or sets the admin user.
        /// </summary>
        public virtual LmsUser AdminUser { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public virtual int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        public virtual string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        public virtual bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the enable proxy tool mode.
        /// </summary>
        public virtual bool? EnableProxyToolMode { get; set; }

        /// <summary>
        /// Gets or sets the proxy tool shared password.
        /// </summary>
        public virtual string ProxyToolSharedPassword { get; set; }

        /// <summary>
        /// Gets or sets the login using cookie.
        /// </summary>
        public virtual bool? LoginUsingCookie { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        public virtual string LmsDomain
        {
            get
            {
                var domainUrl = this.lmsDomain.Return(x => x.ToLower(), string.Empty);

                return domainUrl.RemoveHttpProtocolAndTrailingSlash();
            }

            set
            {
                if (this.UseSSL != true && value.IsSSL())
                {
                    this.UseSSL = true;
                }

                this.lmsDomain = value.Return(x => x.TrimEnd(@"/\".ToCharArray()), null)
                    .RemoveHttpProtocolAndTrailingSlash();
            }
        }

        public virtual int LmsProviderId { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual string PrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the last signal id.
        /// </summary>
        public virtual long LastSignalId { get; set; }

        /// <summary>
        /// Gets or sets the shared secret.
        /// </summary>
        public virtual string SharedSecret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show announcements.
        /// </summary>
        public virtual bool? ShowAnnouncements { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the use user folder.
        /// </summary>
        public virtual bool? UseUserFolder { get; set; }

        /// <summary>
        /// Gets or sets the user folder name.
        /// </summary>
        public virtual string UserFolderName { get; set; }

        /// <summary>
        /// Gets or sets the can remove meeting.
        /// </summary>
        public virtual bool? CanRemoveMeeting { get; set; }

        /// <summary>
        /// Gets or sets the can edit meeting.
        /// </summary>
        public virtual bool? CanEditMeeting { get; set; }

        /// <summary>
        /// Gets or sets the is settings visible.
        /// </summary>
        public virtual bool? IsSettingsVisible { get; set; }

        /// <summary>
        /// Gets or sets the enable office hours.
        /// </summary>
        public virtual bool? EnableOfficeHours { get; set; }

        /// <summary>
        /// Gets or sets the enable study groups.
        /// </summary>
        public virtual bool? EnableStudyGroups { get; set; }

        /// <summary>
        /// Gets or sets the enable course meetings.
        /// </summary>
        public virtual bool? EnableCourseMeetings { get; set; }

        /// <summary>
        /// Gets or sets the show lms help.
        /// </summary>
        public virtual bool? ShowLmsHelp { get; set; }

        /// <summary>
        /// Gets or sets the show egc help.
        /// </summary>
        public virtual bool? ShowEGCHelp { get; set; }

        /// <summary>
        /// Gets or sets the search principal by email first.
        /// </summary>
        public virtual bool? ACUsesEmailAsLogin { get; set; }

        /// <summary>
        /// Gets or sets the add prefix to meeting name.
        /// </summary>
        public virtual bool? AddPrefixToMeetingName { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the LMS users.
        /// </summary>
        public virtual IList<LmsUser> LmsUsers { get; protected set; }

        /// <summary>
        /// Gets or sets the LMS course meetings.
        /// </summary>
        public virtual IList<LmsCourseMeeting> LmsCourseMeetings { get; protected set; }

        public virtual IList<LmsCompanySetting> Settings { get; set; }

        public virtual IList<LmsCompanyRoleMapping> RoleMappings { get; set; }

        /// <summary>
        /// Gets or sets the enable use flv.
        /// </summary>
        public virtual bool UseFLV
        {
            get
            {
                var useFLV = false;
                var setting =
                    this.Settings.SingleOrDefault(x => string.Compare(x.Name, LmsLicenseSettingNames.UseFLV) == 0);
                return setting != null && bool.TryParse(setting.Value, out useFLV) && useFLV;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    this.Settings = new List<LmsCompanySetting>();
                var setting =
                    Settings.SingleOrDefault(x => string.Compare(x.Name, LmsLicenseSettingNames.UseFLV, true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = LmsLicenseSettingNames.UseFLV,
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the enable use mp4.
        /// </summary>
        public virtual bool UseMP4
        {
            get
            {
                var useMP4 = false;
                var setting =
                    this.Settings.SingleOrDefault(x => string.Compare(x.Name, LmsLicenseSettingNames.UseMP4) == 0);
                return setting != null && bool.TryParse(setting.Value, out useMP4) && useMP4;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    this.Settings = new List<LmsCompanySetting>();
                var setting =
                    Settings.SingleOrDefault(x => string.Compare(x.Name, LmsLicenseSettingNames.UseMP4, true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = LmsLicenseSettingNames.UseMP4,
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the condition of multiple meetings .
        /// </summary>
        public virtual bool EnableMultipleMeetings
        {
            get
            {
                var enableMultipleMeetings = false;
                var setting = this.Settings.SingleOrDefault(x =>
                    string.Compare(x.Name, LmsLicenseSettingNames.EnableMultipleMeetings) == 0);
                return setting != null && bool.TryParse(setting.Value, out enableMultipleMeetings) &&
                       enableMultipleMeetings;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    this.Settings = new List<LmsCompanySetting>();
                var setting = Settings.SingleOrDefault(x =>
                    string.Compare(x.Name, LmsLicenseSettingNames.EnableMultipleMeetings, true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = LmsLicenseSettingNames.EnableMultipleMeetings,
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        public virtual bool DenyACUserCreation
        {
            get
            {
                bool denyACUserCreation = false;
                LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                    String.Compare(x.Name, LmsLicenseSettingNames.DenyACUserCreation, true) == 0);
                return setting != null && bool.TryParse(setting.Value, out denyACUserCreation) && denyACUserCreation;
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    Settings = new List<LmsCompanySetting>();

                LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                    string.Compare(x.Name, LmsLicenseSettingNames.DenyACUserCreation, true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = LmsLicenseSettingNames.DenyACUserCreation,
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        public virtual int MeetingNameFormatterId
        {
            get
            {
                LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                    string.Compare(x.Name, LmsLicenseSettingNames.MeetingNameFormatterId, true) == 0);
                if (setting == null)
                    return MeetingNameFormatterFactory.DefaultFormatterId;

                return int.Parse(setting.Value);
            }
            set
            {
                if ((Id == default(int)) && (Settings == null))
                    Settings = new List<LmsCompanySetting>();

                LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                    string.Compare(x.Name, LmsLicenseSettingNames.MeetingNameFormatterId, true) == 0);
                if (setting == null)
                {
                    Settings.Add(new LmsCompanySetting
                    {
                        LmsCompany = this,
                        Name = LmsLicenseSettingNames.MeetingNameFormatterId,
                        Value = value.ToString(),
                    });
                }
                else
                {
                    setting.Value = value.ToString();
                }
            }
        }

        public virtual bool UseSynchronizedUsers
        {
            get
            {
                bool useSynchronizedUsers = false;
                LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                    string.Compare(x.Name, LmsLicenseSettingNames.UseSynchronizedUsers, true) == 0);
                return setting != null && bool.TryParse(setting.Value, out useSynchronizedUsers) &&
                       useSynchronizedUsers;
            }
        }

        public virtual bool EnableMeetingReuse
        {
            get
            {
                bool enableMeetingReuse = false;
                LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                    String.Compare(x.Name, LmsLicenseSettingNames.EnableMeetingReuse, true) == 0);
                return setting != null && bool.TryParse(setting.Value, out enableMeetingReuse) && enableMeetingReuse;
            }
        }

        public virtual string[] AdditionalLmsDomains
        {
            get
            {
                string json = GetSetting<string>(LmsLicenseSettingNames.AdditionalLmsDomains);
                if (string.IsNullOrWhiteSpace(json))
                    return new string[0];
                return JsonConvert.DeserializeObject<string[]>(json);
            }
        }

        public virtual bool CanRemoveRecordings
        {
            get { return GetSetting<bool>(LmsLicenseSettingNames.CanRemoveRecordings, true); }
        }

        public virtual bool AutoPublishRecordings
        {
            get { return GetSetting<bool>(LmsLicenseSettingNames.AutoPublishRecordings, true); }
        }

        public virtual int LanguageId
        {
            get
            {
                return GetSetting<int>(LmsLicenseSettingNames.LanguageId, 5); // 5 = English - default value
            }
        }


        #endregion

        public LmsCompany()
        {
            RoleMappings = new List<LmsCompanyRoleMapping>();
            LmsCourseMeetings = new List<LmsCourseMeeting>();
        }


        // TODO: !!! WWW section!!!
        public virtual bool HasLmsDomain(string domainToCheck)
        {
            var domains = new List<string>();

            if (!string.IsNullOrEmpty(LmsDomain))
                domains.Add(CleanDomain(LmsDomain));

            if ((LmsProviderId == (int) LmsProviderEnum.Blackboard) && EnableProxyToolMode.GetValueOrDefault())
            {
                domains.AddRange(AdditionalLmsDomains.Select(x => x.RemoveHttpProtocolAndTrailingSlash()));
            }

            // NOTE: sakai sends :8080 in our environment- check other LMS
            //index = input.IndexOf(":");
            //if (index > 0)
            //    input = input.Substring(0, index);

            return domains.Any(s => s.Equals(domainToCheck, StringComparison.OrdinalIgnoreCase));
        }

        public virtual T GetSetting<T>(string settingName)
        {
            LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                String.Compare(x.Name, settingName, StringComparison.OrdinalIgnoreCase) == 0);
            return setting == null || string.IsNullOrWhiteSpace(setting.Value)
                ? default(T)
                : (T) Convert.ChangeType(setting.Value, typeof(T)); // assuming that we convert to primitive type
        }

        public virtual T GetSetting<T>(string settingName, T defaultValue)
        {
            LmsCompanySetting setting = Settings.SingleOrDefault(x =>
                String.Compare(x.Name, settingName, StringComparison.OrdinalIgnoreCase) == 0);
            return setting == null || string.IsNullOrWhiteSpace(setting.Value)
                ? defaultValue
                : (T) Convert.ChangeType(setting.Value, typeof(T)); // assuming that we convert to primitive type
        }

        public virtual TelephonyProfileOption GetTelephonyOption(LmsMeetingType meetingType)
        {
            switch (meetingType)
            {
                case LmsMeetingType.Meeting:
                case LmsMeetingType.VirtualClassroom:
                case LmsMeetingType.Seminar:
                    return (TelephonyProfileOption) GetSetting<int>(
                        LmsLicenseSettingNames.Telephony.CourseMeetingOption);

                case LmsMeetingType.OfficeHours:
                    return (TelephonyProfileOption) GetSetting<int>(LmsLicenseSettingNames.Telephony.OfficeHoursOption);

                case LmsMeetingType.StudyGroup:
                    return (TelephonyProfileOption) GetSetting<int>(LmsLicenseSettingNames.Telephony.StudyGroupOption);

                default:
                    throw new ArgumentOutOfRangeException("Non supported meeting type");
            }
        }

        private static string CleanDomain(string input)
        {
            int index = input.IndexOf("/");
            if (index > 0)
                input = input.Substring(0, index);
            return input;
        }

        /// <param name="preferUserSettings">when true - means that user tokens/parameters will be used instead of license(admin)
        /// to retrieve information from API</param>
        public virtual Dictionary<string, object> GetLMSSettings(dynamic settings, LmsUserParameters userParameters = null,
            bool preferUserSettings = false)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            switch (LmsProviderId)
            {
                case (int) LmsProviderEnum.Canvas:
//                        LmsLicenseSettingNames.CanvasOAuthId,
//                        LmsLicenseSettingNames.CanvasOAuthKey


                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    if (userParameters != null)
                    {
                        result.Add(LmsUserSettingNames.Token,
                            preferUserSettings ? userParameters.LmsUser.Token : AdminUser?.Token);
                        result.Add(LmsUserSettingNames.CourseId, userParameters.Course.ToString());
                    }
                    else
                    {
                        result.Add(LmsUserSettingNames.Token, AdminUser?.Token);
                    }
                    //result.Add(LmsUserSettingNames.RefreshToken, session.RefreshToken);
                    break;

                case (int) LmsProviderEnum.AgilixBuzz:
                    result.Add(LmsLicenseSettingNames.BuzzAdminUsername, AdminUser?.Username);
                    result.Add(LmsLicenseSettingNames.BuzzAdminPassword, AdminUser?.Password);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    break;
                case (int)LmsProviderEnum.Schoology:
                    result.Add(LmsLicenseSettingNames.SchoologyConsumerKey, GetSetting<string>(LmsLicenseSettingNames.SchoologyConsumerKey));
                    result.Add(LmsLicenseSettingNames.SchoologyConsumerSecret, GetSetting<string>(LmsLicenseSettingNames.SchoologyConsumerSecret));
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    break;
                case (int) LmsProviderEnum.Blackboard:
                    var enableProxy = EnableProxyToolMode.GetValueOrDefault();
                    result.Add(LmsLicenseSettingNames.UseSSL, UseSSL.GetValueOrDefault());
                    result.Add(LmsLicenseSettingNames.BlackBoardEnableProxyToolMode, enableProxy);
                    if (enableProxy)
                    {
                        string defaultToolRegistrationPassword = settings?.InitialBBPassword;
                        string toolPassword = string.IsNullOrWhiteSpace(ProxyToolSharedPassword)
                            ? defaultToolRegistrationPassword
                            : ProxyToolSharedPassword;
                        result.Add(LmsLicenseSettingNames.BlackBoardProxyToolPassword, toolPassword);
                    }
                    else
                    {
                        result.Add(LmsLicenseSettingNames.AdminUsername, AdminUser?.Username);
                        result.Add(LmsLicenseSettingNames.AdminPassword, AdminUser?.Password);
                    }

                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);

                    //result.Add(LmsLicenseSettingNames.BlackBoardUseSSL, true);

                    if (userParameters != null)
                    {
                        result.Add(LmsUserSettingNames.Token, userParameters.Wstoken);//todo: separate setting?
                        result.Add(LmsUserSettingNames.CourseId, userParameters.Course.ToString());
                        result.Add(LmsUserSettingNames.CourseName, userParameters.CourseName);
                    }
                    
                    break;
                case (int) LmsProviderEnum.Bridge:
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    result.Add(LmsLicenseSettingNames.BridgeApiTokenKey,
                        GetSetting<string>(LmsLicenseSettingNames.BridgeApiTokenKey));
                    result.Add(LmsLicenseSettingNames.BridgeApiTokenSecret,
                        GetSetting<string>(LmsLicenseSettingNames.BridgeApiTokenSecret));
                    break;
                case (int)LmsProviderEnum.Brightspace:
                    result.Add(LmsUserSettingNames.Token, AdminUser?.Token);
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LicenseId, Id);
                    var keys = GetApiKeys(this, (string) settings.BrightspaceAppId,
                        (string) settings.BrightspaceAppKey);
                    result.Add(LmsLicenseSettingNames.BrigthSpaceAppId, keys.Key);
                    result.Add(LmsLicenseSettingNames.BrigthSpaceAppKey, keys.Value);
                    result.Add(LmsLicenseSettingNames.BrightSpaceAllowAdminAdditionToCourse, true);
                    break;
                case (int)LmsProviderEnum.Moodle:
                    result.Add(LmsLicenseSettingNames.MoodleCoreServiceToken, GetSetting<string>(LmsLicenseSettingNames.MoodleCoreServiceToken));
                    result.Add(LmsLicenseSettingNames.MoodleQuizServiceToken, GetSetting<string>(LmsLicenseSettingNames.MoodleQuizServiceToken));
                    result.Add(LmsLicenseSettingNames.UseSSL, UseSSL.GetValueOrDefault());
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    if (AdminUser != null)
                    {
                        result.Add(LmsLicenseSettingNames.AdminUsername, AdminUser.Username);
                        result.Add(LmsLicenseSettingNames.AdminPassword, AdminUser.Password);
                    }
                    if (userParameters != null)
                    {
                        result.Add(LmsUserSettingNames.CourseId, userParameters.Course.ToString());
                        result.Add(LmsUserSettingNames.SessionId, userParameters.Id);
                    }
                    break;
                case (int)LmsProviderEnum.Haiku:
                    result.Add(LmsLicenseSettingNames.HaikuConsumerKey, GetSetting<string>(LmsLicenseSettingNames.HaikuConsumerKey));
                    result.Add(LmsLicenseSettingNames.HaikuConsumerSecret, GetSetting<string>(LmsLicenseSettingNames.HaikuConsumerSecret));
                    result.Add(LmsLicenseSettingNames.HaikuToken, GetSetting<string>(LmsLicenseSettingNames.HaikuToken));
                    result.Add(LmsLicenseSettingNames.HaikuTokenSecret, GetSetting<string>(LmsLicenseSettingNames.HaikuTokenSecret));
                    result.Add(LmsLicenseSettingNames.UseSSL, UseSSL.GetValueOrDefault());
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    break;
                case (int)LmsProviderEnum.Sakai:
                    result.Add(LmsLicenseSettingNames.LicenseKey, ConsumerKey);
                    result.Add(LmsLicenseSettingNames.LicenseSecret, SharedSecret);
                    result.Add(LmsLicenseSettingNames.LanguageId, LanguageId);
                    result.Add(LmsLicenseSettingNames.UseSSL, UseSSL.GetValueOrDefault());
                    result.Add(LmsLicenseSettingNames.LmsDomain, LmsDomain);
                    if (AdminUser != null)
                    {
                        result.Add(LmsLicenseSettingNames.AdminUsername, AdminUser.Username);
                        result.Add(LmsLicenseSettingNames.AdminPassword, AdminUser.Password);
                    }
                    if (userParameters != null)
                    {
                        result.Add(LmsUserSettingNames.CourseId, userParameters.Course.ToString());
                        result.Add(LmsUserSettingNames.CourseName, userParameters.CourseName);
                        result.Add(LmsUserSettingNames.SessionId, userParameters.Id);
                        result.Add(LmsUserSettingNames.UserId, userParameters.LmsUser.UserId);
                        result.Add(LmsUserSettingNames.Username, userParameters.LmsUser.Username);
                    }
                    break;

                default:
                    throw new NotImplementedException($"LmsProviderId {LmsProviderId} is not implemented.");
            }

            return result;
        }

        //todo: move to companyLms/settings service?
        public static KeyValuePair<string, string> GetApiKeys(ILmsLicense lmsCompany, string globalAppId, string globalAppKey)
        {
            string appId = null;
            string appKey = null;
            var isSandbox = lmsCompany.GetSetting<bool>(LmsLicenseSettingNames.IsOAuthSandbox);
            if (isSandbox)
            {
                appId = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.OAuthAppId);
                appKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.OAuthAppKey);
            }
            else
            {
                appId = globalAppId;
                appKey = globalAppKey;
            }

            return new KeyValuePair<string, string>(appId, appKey);
        }
    }

}