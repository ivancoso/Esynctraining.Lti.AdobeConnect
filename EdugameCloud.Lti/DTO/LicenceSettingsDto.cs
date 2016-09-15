using System;
using System.Runtime.Serialization;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Caching;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public sealed class LicenceSettingsDto
    {
        #region Inner Class: LabelsDto

        [DataContract]
        public sealed class LabelsDto
        {
            [DataMember(Name = "meeting")]
            public string MeetingLabel { get; set; }

            [DataMember(Name = "officehour")]
            public string OfficeHourLabel { get; set; }

            [DataMember(Name = "studygroup")]
            public string StudyGroupLabel { get; set; }

            [DataMember(Name = "seminar")]
            public string SeminarLabel { get; set; }

        }

        #endregion Inner Class: LabelsDto

        #region Inner Class: TelephonyDto

        [DataContract]
        public sealed class TelephonyDto
        {
            [DataMember(Name = "activeProfile")]
            public string ActiveProfile { get; set; }

            [DataMember(Name = "courseMeetingOption")]
            public int CourseMeetingOption { get; set; }

            [DataMember(Name = "officeHoursOption")]
            public int OfficeHoursOption { get; set; }

            [DataMember(Name = "studyGroupOption")]
            public int StudyGroupOption { get; set; }

        }

        #endregion Inner Class: TelephonyDto

        [DataMember(Name = "labels")]
        public LabelsDto Labels { get; set; }

        //public string RestoredACPassword { get; set; }

        [DataMember(Name = "acUsesEmailAsLogin")]
        public bool ACUsesEmailAsLogin { get; set; }

        [DataMember(Name = "usesSyncUsers")]
        public bool UseSynchronizedUsers { get; set; }

        [DataMember(Name = "useFLV")]
        public bool UseFLV { get; set; }

        [DataMember(Name = "useMP4")]
        public bool UseMP4 { get; set; }

        [DataMember(Name = "enableMultipleMeetings")]
        public bool EnableMultipleMeetings { get; set; }

        [DataMember(Name = "useSakaiEvents")]
        public bool UseSakaiEvents { get; set; }

        [DataMember(Name = "enableMeetingReuse")]
        public bool EnableMeetingReuse { get; set; }

        [DataMember(Name = "enableSeminars")]
        public bool EnableSeminars { get; set; }

        [DataMember(Name = "enableMyContent")]
        public bool EnableMyContent { get; set; }


        [DataMember(Name = "enableOfficeHours")]
        public bool EnableOfficeHours { get; set; }
        

        [DataMember(Name = "instructionText")]
        public string SupportPageHtml { get; set; }
        
        [DataMember(Name = "showSummary")]
        public bool ShowMeetingSummary { get; set; }
        
        [DataMember(Name = "showTime")]
        public bool ShowMeetingTime { get; set; }
        
        [DataMember(Name = "showDuration")]
        public bool ShowMeetingDuration { get; set; }

        [DataMember(Name = "canRemoveRecordings")]
        public bool CanRemoveRecordings { get; set; }

        [DataMember(Name = "autoPublishRecordings")]
        public bool AutoPublishRecordings { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "hasMp4")]
        public bool HasMp4ServiceLicenseKey { get; set; }
        
        [DataMember(Name = "hasMp4WithSubtitles")]
        public bool HasMp4ServiceWithSubtitlesLicenseKey { get; set; }

        [DataMember(Name = "showAudioProfile")]
        public bool ShowAudioProfile { get; set; }


        [DataMember(Name = "isSettingsVisible")]
        public bool IsSettingsVisible { get; set; }

        [DataMember(Name = "isLmsHelpVisible")]
        public bool IsLmsHelpVisible { get; set; }

        [DataMember(Name = "isEgcHelpVisible")]
        public bool IsEgcHelpVisible { get; set; }

        [DataMember(Name = "canEditMeeting")]
        public bool CanEditMeeting { get; set; }

        [DataMember(Name = "canRemoveMeeting")]
        public bool CanRemoveMeeting { get; set; }

        [DataMember(Name = "canStudentCreateStudyGroup")]
        public bool CanStudentCreateStudyGroup { get; set; }

        [DataMember(Name = "telephony")]
        public TelephonyDto Telephony { get; set; }

        [DataMember(Name = "enableAddGuest")]
        public bool EnableAddGuest { get; set; }

        [DataMember(Name = "enableSetUserRole")]
        public bool EnableSetUserRole { get; set; }

        [DataMember(Name = "enableRemoveUser")]
        public bool EnableRemoveUser { get; set; }


        public static LicenceSettingsDto Build(LmsCompany value, Language lmsLicenseLanguage, ICache cache)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (cache == null)
                throw new ArgumentNullException("cache");

            return CacheUtility.GetCachedItem<LicenceSettingsDto>(cache,
                CachePolicies.Keys.CompanyLmsSettings(value.Id),
                CachePolicies.Dependencies.CompanyLmsSettings(value.Id),
                () =>
            {
                return new LicenceSettingsDto
                {
                    IsSettingsVisible = value.IsSettingsVisible.GetValueOrDefault(),
                    IsLmsHelpVisible = value.ShowLmsHelp.GetValueOrDefault(),
                    IsEgcHelpVisible = value.ShowEGCHelp.GetValueOrDefault(),

                    CanEditMeeting = value.CanEditMeeting.GetValueOrDefault(),
                    CanRemoveMeeting = value.CanRemoveMeeting.GetValueOrDefault(),
                    CanStudentCreateStudyGroup = value.GetSetting<bool>(LmsCompanySettingNames.CanStudentCreateStudyGroup, true),

                    ACUsesEmailAsLogin = value.ACUsesEmailAsLogin ?? false,
                    UseSynchronizedUsers = value.UseSynchronizedUsers,
                    UseFLV = value.UseFLV,
                    UseMP4 = value.UseMP4,
                    EnableMultipleMeetings = value.EnableMultipleMeetings,
                    UseSakaiEvents = value.GetSetting<bool>(LmsCompanySettingNames.UseSakaiEvents),
                    SupportPageHtml = value.GetSetting<string>(LmsCompanySettingNames.SupportPageHtml),

                    EnableMeetingReuse = value.EnableMeetingReuse,

                    EnableSeminars = value.GetSetting<bool>(LmsCompanySettingNames.SeminarsEnable),
                    EnableOfficeHours = value.EnableOfficeHours.GetValueOrDefault(),
                    EnableMyContent = value.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent),
                    EnableAddGuest = value.GetSetting<bool>(LmsCompanySettingNames.EnableAddGuest, true),
                    EnableSetUserRole = value.GetSetting<bool>(LmsCompanySettingNames.EnableSetUserRole, true),
                    EnableRemoveUser = value.GetSetting<bool>(LmsCompanySettingNames.EnableRemoveUser, true),

                    ShowMeetingSummary =  value.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingSummary),
                    ShowMeetingTime = value.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingTime),
                    ShowMeetingDuration = value.GetSetting<bool>(LmsCompanySettingNames.ShowMeetingDuration),

                    CanRemoveRecordings = value.CanRemoveRecordings,
                    AutoPublishRecordings = value.AutoPublishRecordings,
                    Language = lmsLicenseLanguage.TwoLetterCode,

                    HasMp4ServiceLicenseKey = !string.IsNullOrWhiteSpace(value.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey)),
                    HasMp4ServiceWithSubtitlesLicenseKey = !string.IsNullOrWhiteSpace(value.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey)),

                    ShowAudioProfile = value.GetSetting<bool>(LmsCompanySettingNames.ShowAudioProfile),

                    Labels = new LabelsDto
                    {
                        MeetingLabel = value.GetSetting<string>(LmsCompanySettingNames.LabelMeeting) ?? "Course Meetings",
                        OfficeHourLabel = value.GetSetting<string>(LmsCompanySettingNames.LabelOfficeHour) ?? "Office Hours",
                        StudyGroupLabel = value.GetSetting<string>(LmsCompanySettingNames.LabelStudyGroup) ?? "Study Groups",
                        SeminarLabel = value.GetSetting<string>(LmsCompanySettingNames.SeminarsLabel) ?? "Seminar Rooms",
                    },

                    Telephony = new TelephonyDto
                    {
                        ActiveProfile = value.GetSetting<string>(LmsCompanySettingNames.Telephony.ActiveProfile)?.ToUpper(),
                        // See TelephonyProfileOption enum
                        CourseMeetingOption = value.GetSetting<int>(LmsCompanySettingNames.Telephony.CourseMeetingOption),
                        OfficeHoursOption = value.GetSetting<int>(LmsCompanySettingNames.Telephony.OfficeHoursOption),
                        StudyGroupOption = value.GetSetting<int>(LmsCompanySettingNames.Telephony.StudyGroupOption),
                    }
                };
            });            
        }

    }

}
