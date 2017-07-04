using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Meeting;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class LtiViewModelDto
    {
        public class SettingsInfo
        {
            public class ActionUrls
            {
                public static string MyContentBaseUrl { get { return RestWebApiBaseUrl + "my-content/"; } }

                public static string RestWebApiBaseUrl = "";

                public static string Mp4BaseUrl = "../lti-mp4/";

                public static string FileReportBase = "";

                public string GetMP4Recordings { get; set; } = Mp4BaseUrl + "recordings";
                public string ConvertRecording { get; set; } = Mp4BaseUrl + "mp4/convert";
                public string ConvertRecordingWithSubtitles { get; set; } = Mp4BaseUrl + "mp4/convertWithSubtitles";
                public string GetRecordingStatus { get; set; } = Mp4BaseUrl + "recordings/status";

                public string GetRecordings { get; set; } = RestWebApiBaseUrl + "recordings"; // API   + "lti-mp4/recordings"
                public string PublishRecording { get; set; } = RestWebApiBaseUrl + "recordings/publish";  // API
                public string UnpublishRecording { get; set; } = RestWebApiBaseUrl + "recordings/unpublish"; // API

                public string ShareRecording { get; set; } = RestWebApiBaseUrl + "recordings/share";  // API
                public string EditRecordingInformation { get; set; } = RestWebApiBaseUrl + "recordings/edit";  // API
                public string DeleteRecording { get; set; } = RestWebApiBaseUrl + "recordings/delete";  // API

                public string OpenAcRecordingEditPage { get; set; } = "recordings/edit/{0}"; // get
                public string JoinRecording { get; set; } = "recordings/join/{0}";  // get
                public string GetRecordingFLV { get; set; } = "recordings/getFlv/{0}"; // get

                public string GetUsersForMeeting { get; set; } = RestWebApiBaseUrl + "users"; // API
                public string CreateACUser { get; set; } = RestWebApiBaseUrl + "acNewUser"; // API
                public string SearchACUser { get; set; } = RestWebApiBaseUrl + "acSearchUser"; // API
                public string SetDefaultACRoles { get; set; } = RestWebApiBaseUrl + "meeting/SetDefaultACRoles"; // API
                public string SetUserRole { get; set; } = RestWebApiBaseUrl + "users/update"; // API
                public string RemoveUserFromMeeting { get; set; } = RestWebApiBaseUrl + "users/removefrommeeting"; // API

                public string CreateEventsBatch { get; set; } = RestWebApiBaseUrl + "calendar/createBatch"; // API
                public string GetEvents { get; set; } = RestWebApiBaseUrl + "calendar/getevents"; // API
                public string CreateEvent { get; set; } = RestWebApiBaseUrl + "calendar/createevent"; // API
                public string SaveEvent { get; set; } = RestWebApiBaseUrl + "calendar/saveevent"; // API
                public string DeleteEvent { get; set; } = RestWebApiBaseUrl + "calendar/deleteevent"; // API

                public string GetContentShortcuts { get; set; } = MyContentBaseUrl + "shortcuts";
                public string GetFolderContent { get; set; } = MyContentBaseUrl + "content/{0}";
                public string DownloadContent { get; set; } = MyContentBaseUrl + "content/{0}/download";
                public string DeleteContent { get; set; } = MyContentBaseUrl + "content/{0}/delete";
                public string EditContent { get; set; } = MyContentBaseUrl + "content/{0}/edit";
                public string MoveContent { get; set; } = MyContentBaseUrl + "content/{0}/move-to/{1}";
                public string CreateContentFolder { get; set; } = MyContentBaseUrl + "content/{0}/create-sub-folder";

                public string UploadFile { get; set; } = MyContentBaseUrl + "uploading/content/{0}/upload-file";

                public string CheckSettingsPassword { get; set; } = RestWebApiBaseUrl + "settings/checkpass"; // API
                public string SaveSettings { get; set; } = RestWebApiBaseUrl + "settings/save"; // API

                public string JoinToMeeting { get; set; } = "meeting/join";
                public string JoinToMeetingMobile { get; set; } = "meeting/joinmobile";
                public string LeaveMeeting { get; set; } = RestWebApiBaseUrl + "meeting/leave"; // API
                public string DeleteMeeting { get; set; } = RestWebApiBaseUrl + "meeting/delete"; // API
                public string SaveMeetingAndGetUsers { get; set; } = RestWebApiBaseUrl + "meeting/UpdateAndReturnLmsUsers"; // API
                public string SaveMeeting { get; set; } = RestWebApiBaseUrl + "meeting/update";  // API

                public string CourseSections { get; set; } = RestWebApiBaseUrl + "meeting/sections";  // API
                public string UpdateMeetingCourseSection { get; set; } = RestWebApiBaseUrl + "meeting/UpdateMeetingCourseSection";  // API

                public string UseExistingMeeting { get; set; } = RestWebApiBaseUrl + "useExistingMeeting"; // API
                public string GetTemplates { get; set; } = RestWebApiBaseUrl + "templates"; // API
                public string GetAudioProfiles { get; set; } = RestWebApiBaseUrl + "audioProfiles"; // API
                public string SearchACMeetings { get; set; } = RestWebApiBaseUrl + "acSearchMeeting"; // API

                public string SaveSeminar { get; set; } = RestWebApiBaseUrl + "seminars/edit"; // API
                public string CreateSeminar { get; set; } = RestWebApiBaseUrl + "seminars/create"; // API
                public string CreateSeminarSession { get; set; } = RestWebApiBaseUrl + "seminars/sessions/create"; // API
                public string SaveSeminarSession { get; set; } = RestWebApiBaseUrl + "seminars/sessions/edit"; // API
                public string DeleteSeminarSession { get; set; } = RestWebApiBaseUrl + "seminars/sessions/delete";  // API

                public string ReportByRecordings { get; set; } = RestWebApiBaseUrl + "meeting/reports/by-recordings"; // API
                public string ReportBySessions { get; set; } = RestWebApiBaseUrl + "meeting/reports/by-sessions"; // API
                public string ReportByAttendance { get; set; } = RestWebApiBaseUrl + "meeting/reports/by-attendance"; // API

                public string ReportByRecordingsFile { get; set; } = FileReportBase + "meeting-recordings-report"; // lti.host
                public string ReportBySessionsFile { get; set; } = FileReportBase + "meeting-sessions-report"; // lti.host
                public string ReportByAttendanceFile { get; set; } = FileReportBase + "meeting-attendance-report"; // lti.host

            }
        }

        [DataMember]
        public SettingsInfo.ActionUrls ActionUrls { get; set; } = new SettingsInfo.ActionUrls();

        public Version FullVersion { get; set; }

        [DataMember(Name = "version")]
        public string LtiVersion => FullVersion.ToString(3);

        [DataMember(Name = "currentUserName")]
        public string CurrentUserName { get; set; }


        [DataMember(Name = "acSettings")]
        public ACDetailsDTO AcSettings { get; set; }

        [DataMember(Name = "acRoles")]
        public AcRole[] AcRoles
        {
            get
            {
                return new AcRole[] { AcRole.Host, AcRole.Presenter, AcRole.Participant };
            }
            set { }
        }

        [DataMember(Name = "lmsSettings")]
        public LicenseSettingsDto LicenseSettings { get; set; }

        [DataMember(Name = "meetings")]
        public IEnumerable<MeetingDTO> Meetings { get; set; }

        [DataMember(Name = "seminars")]
        public IEnumerable<SeminarLicenseDto> Seminars { get; set; }

        [DataMember(Name = "seminarsMessage")]
        public string SeminarsMessage { get; set; }


        [DataMember(Name = "isTeacher")]
        public bool IsTeacher { get; set; }

        [DataMember(Name = "connectServer")]
        public string ConnectServer { get; set; }

        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }

        [DataMember(Name = "userGuideLink")]
        public string UserGuideLink { get; set; }

        [DataMember(Name = "courseMeetingsEnabled")]
        public bool CourseMeetingsEnabled { get; set; }

        [DataMember(Name = "studyGroupsEnabled")]
        public bool StudyGroupsEnabled { get; set; }

        [DataMember(Name = "syncUsersCountLimit")]
        public int SyncUsersCountLimit { get; set; }

    }

}
