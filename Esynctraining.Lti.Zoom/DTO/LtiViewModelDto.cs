using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.Lti.Zoom.DTO
{
    public class ZoomUrls
    {
        public static string BaseApiUrl { get; set; }
        public static string BaseUrl { get; set; }

        public string GetMeetings => BaseApiUrl + "/meetings"; //get
        public string CreateMeeting => BaseApiUrl + "/meetings"; //post
        public string UpdateMeeting => BaseApiUrl + "/meetings/{meetingId}"; //put
        public string GetMeetingDetails => BaseApiUrl + "/meetings/{meetingId}"; //get
        public string DeleteMeeting => BaseApiUrl + "/meetings/{meetingId}"; //delete
        public string GetMeetingRegistrants => BaseApiUrl + "/meetings/{meetingId}/registrants"; //get
        public string GetSyncParticipants => BaseApiUrl + "/meetings/{meetingId}/syncparticipants"; //get

        public string GetUserInfo => BaseApiUrl + "/users"; //get

        public string GetMeetingReportBySessions =>
            BaseApiUrl + "/reports/meetings/{meetingId}/by-sessions"; //get

        public string GetReportParticipants => BaseApiUrl + "/reports/participants/{sessionId}"; //get

        public string GetRecordings => BaseApiUrl + "/meetings/{meetingId}/recordings"; //get
        public string GetTrashRecordings => BaseApiUrl + "/meetings/{meetingId}/recordings/trash"; //get

        public string DeleteRecordingFile =>
            BaseApiUrl + "/meetings/{meetingId}/recordings/files/{recordingFileId}"; //delete

        public string DeleteRecording =>
            BaseApiUrl + "/meetings/{meetingId}/recordings/{recordingId}"; //delete

        public string RecoverRecordingFile =>
            BaseApiUrl + "/meetings/{meetingId}/recordings/files/{recordingFileId}/status/recover"; //put

        public string RecoverRecording =>
            BaseApiUrl + "/meetings/{meetingId}/recordings/{recordingId}/status/recover"; //put

        public string GetTeacherAlailability => BaseApiUrl + "/office-hours/{meetingId}/availabilities"; //get
        public string AddTeacherAlailability => BaseApiUrl + "/office-hours/{meetingId}/availabilities"; //post
        public string GetSlots => BaseApiUrl + "/office-hours/{meetingId}/slots"; //get
        public string BookSlot => BaseApiUrl + "/office-hours/{meetingId}/slots"; //post
        public string DenySlot => BaseApiUrl + "/office-hours/slots/{slotId}/deny"; //put
        public string DenyDate => BaseApiUrl + "/office-hours/{meetingId}/slots/deny-date"; //put
        public string DenySlotByDate => BaseApiUrl + "/office-hours/{meetingId}/slots/deny"; //put, post
        public string RescheduleSlot => BaseApiUrl + "/office-hours/slots/{slotId}/reschedule"; //put
        public string RescheduleDate => BaseApiUrl + "/office-hours/{meetingId}/reschedule-date"; //put

        //sessions
        public string CreateSessionsBatch { get; set; } = BaseApiUrl + "/meetings/{meetingId}/sessions/createBatch"; // post
        public string GetSessions { get; set; } = BaseApiUrl + "/meetings/{meetingId}/sessions"; // get
        public string CreateSession { get; set; } = BaseApiUrl + "/meetings/{meetingId}/sessions"; // post
        public string SaveSession { get; set; } = BaseApiUrl + "/meetings/{meetingId}/sessions/{sessionId}"; // put
        public string DeleteSession { get; set; } = BaseApiUrl + "/meetings/{meetingId}/sessions/{sessionId}"; // delete

        public string GetKalturaSession => BaseApiUrl + "/kaltura/session"; //get
        //public string UploadKalturaFile => BaseApiUrl + "/meetings/{meetingId}/kaltura-upload"; //post

        public string AddExternalFileRecord => BaseApiUrl + "/meetings/{meetingId}/external-recordings"; //post
        public string DeleteExternalFileRecord => BaseApiUrl + "/meetings/{meetingId}/external-recordings"; //delete

        // web app
        public string JoinMeeting => BaseUrl + "/meetings/{meetingId}/join"; //get
        public string JoinMeetingMobile => BaseUrl + "/meetings/{meetingId}/joinmobile"; //get
        public string DownloadMeetingReportBySessions =>
            BaseUrl + "/reports/meetings/{meetingId}/by-sessions/download"; //get

        public string DownloadMeetingDetailsReport =>
            BaseUrl + "/reports/meetings/{meetingId}/details/{meetingSessionId}/download"; //get
    }

    [DataContract]
    public class LtiViewModelDto
    {
        public class SettingsInfo
        {
            public class ActionUrls
            {
                [DataMember]
                public ZoomUrls Zoom { get; set; } = new ZoomUrls();
            }
        }

        [DataMember]
        public SettingsInfo.ActionUrls ActionUrls { get; set; } = new SettingsInfo.ActionUrls();

        public Version FullVersion { get; set; }

        [DataMember(Name = "version")]
        public string LtiVersion => FullVersion.ToString(3);

        [DataMember(Name = "currentUserName")]
        public string CurrentUserName { get; set; }

        //[DataMember(Name = "licenseSettings")]
        //public LicenseSettingsDto LicenseSettings { get; set; }


        [DataMember(Name = "isTeacher")]
        public bool IsTeacher { get; set; }

        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }

        [DataMember(Name = "userGuideLink")]
        public string UserGuideLink { get; set; }

        [DataMember(Name = "courseMeetingsEnabled")]
        public bool CourseMeetingsEnabled { get; set; }

        public List<int> EnabledStorageProviders { get; set; }
        public bool EnableOfficeHours { get; set; }
        public bool EnableStudyGroups { get; set; }
        public bool EnableClassRosterSecurity { get; set; }
        public string PrimaryColor { get; set; }
        public string SupportSectionText { get; set; }
    }
}
