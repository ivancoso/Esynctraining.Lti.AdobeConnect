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
                public static string RestWebApiBaseUrl = "";

                public string GetMP4Recordings { get; set; } = "lti-mp4/recordings";
                public string ConvertRecording { get; set; } = "lti-mp4/mp4/convert";
                public string ConvertRecordingWithSubtitles { get; set; } = "lti-mp4/mp4/convertWithSubtitles";
                public string GetRecordingStatus { get; set; } = "lti-mp4/recordings/status";

                public string GetRecordings { get; set; } = RestWebApiBaseUrl + "recordings"; // API   + "lti-mp4/recordings"
                public string PublishRecording { get; set; } = RestWebApiBaseUrl + "recordings/publish";  // API
                public string UnpublishRecording { get; set; } = RestWebApiBaseUrl + "recordings/unpublish"; // API

                public string ShareRecording { get; set; } = RestWebApiBaseUrl + "recordings/share";  // API
                public string EditRecordingInformation { get; set; } = RestWebApiBaseUrl + "recordings/edit";  // API
                public string DeleteRecording { get; set; } = RestWebApiBaseUrl + "recordings/delete/{0}";  // API

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

                public string GetContentShortcuts { get; set; } = "../lti-my-content/{0}/shortcuts";
                public string GetFolderContent { get; set; } = "../lti-my-content/{0}/content/{1}";
                public string DownloadContent { get; set; } = "../lti-my-content/{0}/content/{1}/download";
                public string DeleteContent { get; set; } = "../lti-my-content/{0}/content/{1}/delete";
                public string EditContent { get; set; } = "../lti-my-content/{0}/content/{1}/edit";
                public string MoveContent { get; set; } = "../lti-my-content/{0}/content/{1}/move-to/{2}";
                public string CreateContentFolder { get; set; } = "../lti-my-content/{0}/content/{1}/create-sub-folder";

                public string CheckSettingsPasword { get; set; } = RestWebApiBaseUrl + "settings/checkpass"; // API
                public string SaveSettings { get; set; } = RestWebApiBaseUrl + "settings/save"; // API

                public string JoinToMeeting { get; set; } = "meeting/join";
                public string JoinToMeetingMobile { get; set; } = "meeting/joinmobile";
                public string LeaveMeeting { get; set; } = RestWebApiBaseUrl + "meeting/leave"; // API
                public string DeleteMeeting { get; set; } = RestWebApiBaseUrl + "meeting/delete"; // API
                public string SaveMeetingAndGetUsers { get; set; } = RestWebApiBaseUrl + "meeting/UpdateAndReturnLmsUsers"; // API
                public string SaveMeeting { get; set; } = RestWebApiBaseUrl + "meeting/Update";  // API

                public string UseExistingMeeting { get; set; } = RestWebApiBaseUrl + "useExistingMeeting"; // API
                public string GetTemplates { get; set; } = RestWebApiBaseUrl + "templates"; // API
                public string GetAudioProfiles { get; set; } = RestWebApiBaseUrl + "AudioProfiles"; // API
                public string SearchACMeetings { get; set; } = RestWebApiBaseUrl + "acSearchMeeting"; // API

                public string SaveSeminar { get; set; } = RestWebApiBaseUrl + "seminars/edit"; // API
                public string CreateSeminar { get; set; } = RestWebApiBaseUrl + "seminars/create"; // API
                public string CreateSeminarSession { get; set; } = RestWebApiBaseUrl + "seminars/sessions/create"; // API
                public string SaveSeminarSession { get; set; } = RestWebApiBaseUrl + "seminars/sessions/edit"; // API
                public string DeleteSeminarSession { get; set; } = RestWebApiBaseUrl + "seminars/sessions/delete";  // API

                public string ReportByRecordings { get; set; } = RestWebApiBaseUrl + "meeting/reports/by-recordings"; // API
                public string ReportBySessions { get; set; } = RestWebApiBaseUrl + "meeting/reports/by-sessions"; // API
                public string ReportByAttendance { get; set; } = RestWebApiBaseUrl + "meeting/reports/by-attendance"; // API

                public string ReportByRecordingsFile { get; set; } = "meeting-recordings-report"; // lti.host
                public string ReportBySessionsFile { get; set; } = "meeting-sessions-report"; // lti.host
                public string ReportByAttendanceFile { get; set; } = "meeting-attendance-report"; // lti.host

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
        public LicenceSettingsDto LicenceSettings { get; set; }

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
