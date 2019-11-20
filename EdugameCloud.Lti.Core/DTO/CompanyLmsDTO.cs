using DocumentFormat.OpenXml.Drawing.Charts;
using EdugameCloud.Lti.Core.Constants;
using Esynctraining.Lti.Lms.Common.Constants;

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
        private TelephonyDTO _telephony;


        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsDTO"/> class.
        /// </summary>
        public CompanyLmsDTO()
        {
            roleMapping = new LmsCompanyRoleMappingDTO[0];
            additionalLmsDomains = new string[0];
            Telephony = new TelephonyDTO();
            enableAddGuest = true;
            enableSetUserRole = true;
            enableRemoveUser = true;
        }

        public CompanyLmsDTO(LmsCompany instance, LmsProvider provider, dynamic settings)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

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
            canStudentCreateStudyGroup = instance.GetSetting<bool>(LmsLicenseSettingNames.CanStudentCreateStudyGroup, true);
            this.isSettingsVisible = instance.IsSettingsVisible.GetValueOrDefault();
            this.enableOfficeHours = instance.EnableOfficeHours.GetValueOrDefault();
            this.enableStudyGroups = instance.EnableStudyGroups.GetValueOrDefault();
            this.enableCourseMeetings = instance.EnableCourseMeetings.GetValueOrDefault();

            this.enableVirtualClassrooms = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableVirtualClassrooms, false);
            this.namedVirtualClassroomManager = instance.GetSetting<bool>(LmsLicenseSettingNames.NamedVirtualClassroomManager, true);
            this.labelVirtualClassroom = instance.GetSetting<string>(LmsLicenseSettingNames.VirtualClassroomsLabel);

            this.showEGCHelp = instance.ShowEGCHelp.GetValueOrDefault();
            this.showLmsHelp = instance.ShowLmsHelp.GetValueOrDefault();
            this.addPrefixToMeetingName = instance.AddPrefixToMeetingName.GetValueOrDefault();
            this.userFolderName = instance.UserFolderName;

            Uri portalUrl = new Uri((string)settings.PortalUrl, UriKind.Absolute);
            this.setupUrl = new Uri(portalUrl, $"content/lti-config/{provider.LmsProviderName}.xml").ToString();

            this.enableProxyToolMode = instance.EnableProxyToolMode ?? false;
            this.proxyToolPassword = instance.ProxyToolSharedPassword;

            this.allowUserCreation = !instance.DenyACUserCreation;
            this.showAuthToken = !instance.LoginUsingCookie.GetValueOrDefault();
            this.acUsesEmailAsLogin = instance.ACUsesEmailAsLogin.GetValueOrDefault();
            this.enableAnnouncements = instance.ShowAnnouncements.GetValueOrDefault();
            this.useSynchronizedUsers = instance.UseSynchronizedUsers;
            this.meetingNameFormatterId = instance.MeetingNameFormatterId;
            this.roleMapping = instance.RoleMappings.Select(x =>
                new LmsCompanyRoleMappingDTO(x.LmsRoleName, x.AcRole, x.IsDefaultLmsRole, x.IsTeacherRole)).ToArray();
            this.isSandbox = instance.GetSetting<bool>(LmsLicenseSettingNames.IsOAuthSandbox);
            this.oAuthAppId = instance.GetSetting<string>(LmsLicenseSettingNames.OAuthAppId);
            this.oAuthAppKey = instance.GetSetting<string>(LmsLicenseSettingNames.OAuthAppKey);
            this.supportPageHtml = instance.GetSetting<string>(LmsLicenseSettingNames.SupportPageHtml);
            this.isActive = instance.IsActive;

            this.labelMeeting = instance.GetSetting<string>(LmsLicenseSettingNames.LabelMeeting);
            this.labelOfficeHour = instance.GetSetting<string>(LmsLicenseSettingNames.LabelOfficeHour);
            this.labelStudyGroup = instance.GetSetting<string>(LmsLicenseSettingNames.LabelStudyGroup);

            this.enableMeetingReuse = instance.EnableMeetingReuse;
            this.additionalLmsDomains = instance.AdditionalLmsDomains;

            this.showSummary = instance.GetSetting<bool>(LmsLicenseSettingNames.ShowMeetingSummary);
            this.showTime = instance.GetSetting<bool>(LmsLicenseSettingNames.ShowMeetingTime);
            this.showDuration = instance.GetSetting<bool>(LmsLicenseSettingNames.ShowMeetingDuration);

            this.canRemoveRecordings = instance.CanRemoveRecordings;
            this.autoPublishRecordings = instance.AutoPublishRecordings;
            this.forcedAddInInstallation = instance.GetSetting<bool>(LmsLicenseSettingNames.ForcedAddInInstallation);

            this.languageId = instance.LanguageId;

            this.mp4ServiceLicenseKey = instance.GetSetting<string>(LmsLicenseSettingNames.Mp4ServiceLicenseKey);
            this.mp4ServiceWithSubtitlesLicenseKey = instance.GetSetting<string>(LmsLicenseSettingNames.Mp4ServiceWithSubtitlesLicenseKey);

            this.showAudioProfile = instance.GetSetting<bool>(LmsLicenseSettingNames.ShowAudioProfile);
            this.audioProfileUnique = instance.GetSetting<bool>(LmsLicenseSettingNames.AudioProfileUnique);
            this.enableSeminars = instance.GetSetting<bool>(LmsLicenseSettingNames.SeminarsEnable);
            this.labelSeminar = instance.GetSetting<string>(LmsLicenseSettingNames.SeminarsLabel);
            this.enableAuditGuestEntry = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableAuditGuestEntry);
            this.HidePrivateRecordingsForStudents = instance.GetSetting<bool>(LmsLicenseSettingNames.HidePrivateRecordingsForStudents);
            useSakaiEvents = instance.GetSetting<bool>(LmsLicenseSettingNames.UseSakaiEvents);
            enableMeetingSessions = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableMeetingSessions);
            enableMyContent = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableMyContent);
            enableAddGuest = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableAddGuest, true);
            enableSetUserRole = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableSetUserRole, true);
            enableRemoveUser = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableRemoveUser, true);
            moodleCoreServiceToken = instance.GetSetting<string>(LmsLicenseSettingNames.MoodleCoreServiceToken);
            moodleQuizServiceToken = instance.GetSetting<string>(LmsLicenseSettingNames.MoodleQuizServiceToken);
            schoologyConsumerKey = instance.GetSetting<string>(LmsLicenseSettingNames.SchoologyConsumerKey);
            schoologyConsumerSecret = instance.GetSetting<string>(LmsLicenseSettingNames.SchoologyConsumerSecret);
            haikuConsumerKey = instance.GetSetting<string>(LmsLicenseSettingNames.HaikuConsumerKey);
            haikuConsumerSecret = instance.GetSetting<string>(LmsLicenseSettingNames.HaikuConsumerSecret);
            haikuToken = instance.GetSetting<string>(LmsLicenseSettingNames.HaikuToken);
            haikuTokenSecret = instance.GetSetting<string>(LmsLicenseSettingNames.HaikuTokenSecret);
            isPdfMeetingUrl = instance.GetSetting<bool>(LmsLicenseSettingNames.IsPdfMeetingUrl);
            bridgeApiTokenKey = instance.GetSetting<string>(LmsLicenseSettingNames.BridgeApiTokenKey);
            bridgeApiTokenSecret = instance.GetSetting<string>(LmsLicenseSettingNames.BridgeApiTokenSecret);
            UseCourseSections = instance.GetSetting<bool>(LmsLicenseSettingNames.UseCourseSections);
            UseCourseMeetingsCustomLayout = instance.GetSetting<bool>(LmsLicenseSettingNames.UseCourseMeetingsCustomLayout);
            EnableOfficeHoursSlots = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableOfficeHoursSlots);
            EnableCanvasExportToCalendar = instance.GetSetting<bool>(LmsLicenseSettingNames.EnableCanvasExportToCalendar);
            Telephony = new TelephonyDTO(instance);
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

        [DataMember]
        public bool canStudentCreateStudyGroup { get; set; }

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

        [DataMember]
        public bool enableVirtualClassrooms { get; set; }

        [DataMember]
        public bool namedVirtualClassroomManager { get; set; }

        [DataMember]
        public string labelVirtualClassroom { get; set; }

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
        public string oAuthAppId { get; set; }

        [DataMember]
        public string oAuthAppKey { get; set; }

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

        [DataMember]
        public bool showAudioProfile { get; set; }

        [DataMember]
        public bool? audioProfileUnique { get; set; }

        [DataMember]
        public bool enableSeminars { get; set; }

        [DataMember]
        public string labelSeminar { get; set; }

        [DataMember]
        public bool enableAuditGuestEntry { get; set; }

        [DataMember]
        public bool enableMeetingSessions { get; set; }

        [DataMember]
        public bool useSakaiEvents { get; set; }

        [DataMember(Name = "telephony")]
        public TelephonyDTO Telephony
        {
            get { return _telephony; }
            set { _telephony = value ?? new TelephonyDTO(); }
        }

        [DataMember]
        public bool enableMyContent { get; set; }

        [DataMember]
        public bool enableAddGuest { get; set; }

        [DataMember]
        public bool enableSetUserRole { get; set; }

        [DataMember]
        public bool enableRemoveUser { get; set; }

        [DataMember]
        public string moodleCoreServiceToken { get; set; }
        [DataMember]
        public string moodleQuizServiceToken { get; set; }

        [DataMember]
        public string schoologyConsumerKey { get; set; }
        [DataMember]
        public string schoologyConsumerSecret { get; set; }

        [DataMember]
        public string haikuConsumerKey { get; set; }
        [DataMember]
        public string haikuConsumerSecret { get; set; }
        [DataMember]
        public string haikuToken { get; set; }
        [DataMember]
        public string haikuTokenSecret { get; set; }

        [DataMember]
        public string bridgeApiTokenKey { get; set; }
        [DataMember]
        public string bridgeApiTokenSecret { get; set; }

        [DataMember]
        public bool isPdfMeetingUrl { get; set; }

        [DataMember(Name = "useUserSections")]
        public bool UseCourseSections { get; set; }

        [DataMember(Name = "useCourseMeetingsCustomLayout")]
        public bool UseCourseMeetingsCustomLayout { get; set; }

        [DataMember(Name = "enableOfficeHoursSlots")]
        public bool EnableOfficeHoursSlots { get; set; }

        [DataMember(Name = "enableCanvasExportToCalendar")]
        public bool EnableCanvasExportToCalendar { get; set; }

        [DataMember(Name = "hidePrivateRecordingsForStudents")]
        public bool HidePrivateRecordingsForStudents { get; set; }
    }

}
