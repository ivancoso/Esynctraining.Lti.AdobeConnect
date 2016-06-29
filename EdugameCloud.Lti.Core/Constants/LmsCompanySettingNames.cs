namespace EdugameCloud.Lti.Core.Constants
{
    public class LmsCompanySettingNames
    {
        public const string IsD2LSandbox = "IsD2LSandbox";
        public const string D2LAppId = "D2LAppId";
        public const string D2LAppKey = "D2LAppKey";
        public const string UseSynchronizedUsers = "UseSynchronizedUsers";
        public const string SupportPageHtml = "SupportPageHtml";
        public const string LabelMeeting = "LabelMeeting";
        public const string LabelOfficeHour = "LabelOfficeHour";
        public const string LabelStudyGroup = "LabelStudyGroup";
        public const string EnableMeetingReuse = "EnableMeetingReuse";
        public const string MeetingNameFormatterId = "MeetingNameFormatterId";
        public const string DenyACUserCreation = "DenyACUserCreation";
        public const string EnableMultipleMeetings = "EnableMultipleMeetings";
        public const string UseMP4 = "UseMP4";
        public const string UseFLV = "UseFLV";
        public const string AdditionalLmsDomains = "AdditionalLmsDomains";
        public const string ShowMeetingSummary = "ShowMeetingSummary";
        public const string ShowMeetingTime = "ShowMeetingTime";
        public const string ShowMeetingDuration = "ShowMeetingDuration";
        public const string CanRemoveRecordings = "CanRemoveRecordings";
        public const string AutoPublishRecordings = "AutoPublishRecordings";
        public const string ForcedAddInInstallation = "ForcedAddInInstallation";
        public const string LanguageId = "LanguageId";
        public const string Mp4ServiceLicenseKey = "Mp4ServiceLicenseKey";
        public const string Mp4ServiceWithSubtitlesLicenseKey = "Mp4ServiceWithSubtitlesLicenseKey";
        public const string ShowAudioProfile = "ShowAudioProfile";
        public const string AudioProfileUnique = "AudioProfileUnique";

        public const string SeminarsEnable = "SeminarsEnable";
        public const string SeminarsLabel = "SeminarsLabel";

        public const string EnableAuditGuestEntry = "EnableAuditGuestEntry";
        public const string UseSakaiEvents = "UseSakaiEvents";
        public const string EnableMyContent = nameof(EnableMyContent);
        public const string EnableAddGuest = nameof(EnableAddGuest);
        public const string EnableSetUserRole = nameof(EnableSetUserRole);
        public const string EnableRemoveUser = nameof(EnableRemoveUser);
        
        public static class Telephony
        {
            public const string ActiveProfile = "Telephony.ActiveProfile";  // None / MeetingOne / Arkadin

            public const string CourseMeetingOption = "Telephony.CourseMeetingOption";

            public const string OfficeHoursOption = "Telephony.OfficeHoursOption";

            public const string StudyGroupOption = "Telephony.StudyGroupOption";

            public static class MeetingOne
            {
                public const string UserName = "Telephony.MeetingOne.UserName";

                public const string SecretHashKey = "Telephony.MeetingOne.SecretHashKey";

                public const string OwningAccountNumber = "Telephony.MeetingOne.OwningAccountNumber";

            }

            public static class Arkadin
            {
                public const string UserName = "Telephony.Arkadin.UserName";

            }


        }

    }

}