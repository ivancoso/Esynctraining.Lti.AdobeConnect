using EdugameCloud.Core.Extensions;
using EdugameCloud.Lti.Core.Constants;

namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.Extensions;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The company LMS DTO.
    /// </summary>
    [DataContract]
    public sealed class CompanyLmsDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsDTO"/> class.
        /// </summary>
        public CompanyLmsDTO()
        {
            roleMapping = new LmsCompanyRoleMappingDTO[0];
            additionalLmsDomains = new string[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsDTO"/> class.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public CompanyLmsDTO(LmsCompany instance, LmsProvider provider)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.id = instance.Id;
            this.useFLV = instance.UseFLV;
            this.useMP4 = instance.UseMP4;
            this.enableMultipleMeetings = instance.EnableMultipleMeetings;
            this.acServer = instance.AcServer;
            this.acUsername = instance.AcUsername;
            this.companyId = instance.CompanyId.Return(x => x, 0);
            this.consumerKey = instance.ConsumerKey;
            this.createdBy = instance.CreatedBy.Return(x => x, 0);
            this.modifiedBy = instance.ModifiedBy.Return(x => x.Value, 0);
            this.dateCreated = instance.DateCreated.ConvertToUnixTimestamp();
            this.dateModified = instance.DateModified.ConvertToUnixTimestamp();

            this.lmsProvider = provider.Return(x => x.ShortName, string.Empty);
            this.sharedSecret = instance.SharedSecret;
            this.lmsAdmin = instance.AdminUser.With(x => x.Username);
            this.lmsAdminToken = instance.AdminUser.With(x => x.Token);
            this.lmsDomain = instance.LmsDomain.AddHttpProtocol(instance.UseSSL.GetValueOrDefault());
            this.primaryColor = instance.PrimaryColor;
            this.title = instance.Title;
            this.useUserFolder = instance.UseUserFolder.GetValueOrDefault();
            this.canRemoveMeeting = instance.CanRemoveMeeting.GetValueOrDefault();
            this.canEditMeeting = instance.CanEditMeeting.GetValueOrDefault();
            this.isSettingsVisible = instance.IsSettingsVisible.GetValueOrDefault();
            this.enableOfficeHours = instance.EnableOfficeHours.GetValueOrDefault();
            this.enableStudyGroups = instance.EnableStudyGroups.GetValueOrDefault();
            this.enableCourseMeetings = instance.EnableCourseMeetings.GetValueOrDefault();
            this.showEGCHelp = instance.ShowEGCHelp.GetValueOrDefault();
            this.showLmsHelp = instance.ShowLmsHelp.GetValueOrDefault();
            this.addPrefixToMeetingName = instance.AddPrefixToMeetingName.GetValueOrDefault();
            this.userFolderName = instance.UserFolderName;

            this.setupUrl = provider != null ?
                (!string.IsNullOrWhiteSpace(provider.ConfigurationUrl) ? provider.ConfigurationUrl
                : provider.ShortName != null ? string.Format("/Content/lti-config/{0}.xml", provider.ShortName) : null)
                : null;

            this.enableProxyToolMode = instance.EnableProxyToolMode ?? false;
            this.proxyToolPassword = instance.ProxyToolSharedPassword;

            this.allowUserCreation = !instance.DenyACUserCreation;
            this.showAuthToken = !instance.LoginUsingCookie.GetValueOrDefault();
            this.acUsesEmailAsLogin = instance.ACUsesEmailAsLogin.GetValueOrDefault();
            this.enableAnnouncements = instance.ShowAnnouncements.GetValueOrDefault();
            this.useSynchronizedUsers = instance.UseSynchronizedUsers;
            this.meetingNameFormatterId = instance.MeetingNameFormatterId;
            this.roleMapping = instance.RoleMappings.Select(x => new LmsCompanyRoleMappingDTO(x.LmsRoleName, x.AcRole, x.IsDefaultLmsRole)).ToArray();
            this.isSandbox = instance.GetSetting<bool>(LmsCompanySettingNames.IsD2LSandbox);
            this.d2lAppId = instance.GetSetting<string>(LmsCompanySettingNames.D2LAppId);
            this.d2lAppKey = instance.GetSetting<string>(LmsCompanySettingNames.D2LAppKey);
            this.supportPageHtml = instance.GetSetting<string>(LmsCompanySettingNames.SupportPageHtml);
            this.isActive = instance.IsActive;

            this.labelMeeting = instance.GetSetting<string>(LmsCompanySettingNames.LabelMeeting);
            this.labelOfficeHour = instance.GetSetting<string>(LmsCompanySettingNames.LabelOfficeHour);
            this.labelStudyGroup = instance.GetSetting<string>(LmsCompanySettingNames.LabelStudyGroup);
            this.enableMeetingReuse = instance.EnableMeetingReuse;
            this.additionalLmsDomains = instance.AdditionalLmsDomains;

            this.showSummary = instance.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingSummary);
            this.showTime = instance.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingTime);
            this.showDuration = instance.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingDuration);

            this.canRemoveRecordings = instance.CanRemoveRecordings;
            this.autoPublishRecordings = instance.AutoPublishRecordings;
            this.forcedAddInInstallation = instance.GetSetting<bool>(LmsCompanySettingNames.ForcedAddInInstallation);

            this.languageId = instance.LanguageId;

            this.mp4ServiceLicenseKey = instance.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
            this.mp4ServiceWithSubtitlesLicenseKey = instance.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
        }

        /// <summary>
        /// Gets or sets the condition of multiple meetings.
        /// </summary>
        [DataMember]
        public bool enableMultipleMeetings { get; set; }

        [DataMember]
        public bool enableMeetingReuse { get; set; }

        /// <summary>
        /// Gets or sets the enable use MP4.
        /// </summary>
        [DataMember]
        public bool useMP4 { get; set; }

        /// <summary>
        /// Gets or sets the enable use FLV.
        /// </summary>
        [DataMember]
        public bool useFLV { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the LMS admin.
        /// </summary>
        [DataMember]
        public string lmsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        [DataMember]
        public string lmsDomain { get; set; }

        /// <summary>
        /// Gets or sets the LMS admin password.
        /// </summary>
        [DataMember]
        public string lmsAdminPassword { get; set; }

        /// <summary>
        /// Gets or sets the proxy tool mode.
        /// </summary>
        [DataMember]
        public bool enableProxyToolMode { get; set; }

        /// <summary>
        /// Gets or sets the proxy tool password.
        /// </summary>
        [DataMember]
        public string proxyToolPassword { get; set; }

        /// <summary>
        /// Gets or sets the LMS admin token.
        /// </summary>
        [DataMember]
        public string lmsAdminToken { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider.
        /// </summary>
        [DataMember]
        public string lmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the AC server.
        /// </summary>
        [DataMember]
        public string acServer { get; set; }

        /// <summary>
        /// Gets or sets the AC username.
        /// </summary>
        [DataMember]
        public string acUsername { get; set; }

        /// <summary>
        /// Gets or sets the AC password.
        /// </summary>
        [DataMember]
        public string acPassword { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        [DataMember]
        public string consumerKey { get; set; }

        /// <summary>
        /// Gets or sets the shared secret.
        /// </summary>
        [DataMember]
        public string sharedSecret { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        [DataMember]
        public string primaryColor { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DataMember]
        public string title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use user folder.
        /// </summary>
        [DataMember]
        public bool useUserFolder { get; set; }

        /// <summary>
        /// Gets or sets the user folder name.
        /// </summary>
        [DataMember]
        public string userFolderName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can remove meeting.
        /// </summary>
        [DataMember]
        public bool canRemoveMeeting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can edit meeting.
        /// </summary>
        [DataMember]
        public bool canEditMeeting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is settings visible.
        /// </summary>
        [DataMember]
        public bool isSettingsVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable office hours.
        /// </summary>
        [DataMember]
        public bool enableOfficeHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable study groups.
        /// </summary>
        [DataMember]
        public bool enableStudyGroups { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable course meetings.
        /// </summary>
        [DataMember]
        public bool enableCourseMeetings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show lms help.
        /// </summary>
        [DataMember]
        public bool showLmsHelp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show egc help.
        /// </summary>
        [DataMember]
        public bool showEGCHelp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether add prefix to meeting name.
        /// </summary>
        [DataMember]
        public bool addPrefixToMeetingName { get; set; }

        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the setup url.
        /// </summary>
        [DataMember]
        public string setupUrl { get; set; }

        [DataMember]
        public bool allowUserCreation { get; set; }

        [DataMember]
        public bool showAuthToken { get; set; }

        [DataMember]
        public bool acUsesEmailAsLogin { get; set; }

        /// <summary>
        /// If true, enables announcement creation in LMS when new course meetings is being created
        /// </summary>
        [DataMember]
        public bool enableAnnouncements { get; set; }

        [DataMember]
        public bool useSynchronizedUsers { get; set; }

        [DataMember]
        public int meetingNameFormatterId { get; set; }

        [DataMember]
        public LmsCompanyRoleMappingDTO[] roleMapping { get; set; }

        [DataMember]
        public bool? isSandbox { get; set; }

        [DataMember]
        public string d2lAppId { get; set; }

        [DataMember]
        public string d2lAppKey { get; set; }

        [DataMember]
        public string supportPageHtml { get; set; }

        [DataMember]
        public string labelMeeting { get; set; }

        [DataMember]
        public string labelOfficeHour { get; set; }

        [DataMember]
        public string labelStudyGroup { get; set; }

        [DataMember]
        public string[] additionalLmsDomains { get; set; }

        [DataMember]
        public bool showSummary { get; set; }

        [DataMember]
        public bool showTime { get; set; }

        [DataMember]
        public bool showDuration { get; set; }

        [DataMember]
        public bool canRemoveRecordings { get; set; }

        [DataMember]
        public bool autoPublishRecordings { get; set; }

        [DataMember]
        public bool forcedAddInInstallation { get; set; }

        [DataMember]
        public int languageId { get; set; }


        [DataMember]
        public string mp4ServiceLicenseKey { get; set; }

        [DataMember]
        public string mp4ServiceWithSubtitlesLicenseKey { get; set; }

    }

}
