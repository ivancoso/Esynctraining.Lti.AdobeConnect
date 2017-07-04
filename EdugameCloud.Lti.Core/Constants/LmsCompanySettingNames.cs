namespace EdugameCloud.Lti.Core.Constants
{
    public class LmsCompanySettingNames
    {
        public const string IsOAuthSandbox = nameof(IsOAuthSandbox);
        public const string OAuthAppId = nameof(OAuthAppId);
        public const string OAuthAppKey = nameof(OAuthAppKey);
        public const string UseSynchronizedUsers = nameof(UseSynchronizedUsers);
        public const string SupportPageHtml = nameof(SupportPageHtml);
        public const string LabelMeeting = nameof(LabelMeeting);
        public const string LabelOfficeHour = nameof(LabelOfficeHour);
        public const string LabelStudyGroup = nameof(LabelStudyGroup);
        public const string EnableMeetingReuse = nameof(EnableMeetingReuse);
        public const string MeetingNameFormatterId = nameof(MeetingNameFormatterId);
        public const string DenyACUserCreation = nameof(DenyACUserCreation);
        public const string EnableMultipleMeetings = nameof(EnableMultipleMeetings);
        public const string UseMP4 = nameof(UseMP4);
        public const string UseFLV = nameof(UseFLV);
        public const string AdditionalLmsDomains = nameof(AdditionalLmsDomains);
        public const string ShowMeetingSummary = nameof(ShowMeetingSummary);
        public const string ShowMeetingTime = nameof(ShowMeetingTime);
        public const string ShowMeetingDuration = nameof(ShowMeetingDuration);
        public const string CanRemoveRecordings = nameof(CanRemoveRecordings);
        public const string AutoPublishRecordings = nameof(AutoPublishRecordings);
        public const string ForcedAddInInstallation = nameof(ForcedAddInInstallation);
        public const string LanguageId = nameof(LanguageId);
        public const string Mp4ServiceLicenseKey = nameof(Mp4ServiceLicenseKey);
        public const string Mp4ServiceWithSubtitlesLicenseKey = nameof(Mp4ServiceWithSubtitlesLicenseKey);
        public const string ShowAudioProfile = nameof(ShowAudioProfile);
        public const string AudioProfileUnique = nameof(AudioProfileUnique);

        public const string SeminarsEnable = nameof(SeminarsEnable);
        public const string SeminarsLabel = nameof(SeminarsLabel);

        public const string EnableAuditGuestEntry = nameof(EnableAuditGuestEntry);
        public const string UseSakaiEvents = nameof(UseSakaiEvents);
        public const string EnableMyContent = nameof(EnableMyContent);
        public const string EnableAddGuest = nameof(EnableAddGuest);
        public const string EnableSetUserRole = nameof(EnableSetUserRole);
        public const string EnableRemoveUser = nameof(EnableRemoveUser);
        public const string MoodleCoreServiceToken = nameof(MoodleCoreServiceToken);
        public const string MoodleQuizServiceToken = nameof(MoodleQuizServiceToken);
        public const string CanStudentCreateStudyGroup = nameof(CanStudentCreateStudyGroup);
        public const string EnableMeetingSessions = nameof(EnableMeetingSessions);
        public const string UseCourseSections = nameof(UseCourseSections);
        public const string UseCourseMeetingsCustomLayout = nameof(UseCourseMeetingsCustomLayout);

        public const string EnableVirtualClassrooms = nameof(EnableVirtualClassrooms);
        public const string VirtualClassroomsLabel = nameof(VirtualClassroomsLabel);

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

        public static string IsPdfMeetingUrl = nameof(IsPdfMeetingUrl);
    }

}