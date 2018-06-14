using System;
using System.Runtime.Serialization;

namespace Esynctraining.Lti.Zoom.DTO
{
    public class ZoomUrls
    {
        public static string BaseUrl { get; set; }

        public string JoinMeeting => BaseUrl + "/meetings/{meetingId}/join"; //get

        public string GetMeetings => BaseUrl + "/meetings"; //get
        public string CreateMeeting => BaseUrl + "/meetings"; //post
        public string UpdateMeeting => BaseUrl + "/meetings/{meetingId}"; //put
        public string GetMeetingDetails => BaseUrl + "/meetings/{meetingId}"; //get
        public string DeleteMeeting => BaseUrl + "/meetings/{meetingId}"; //delete

        public string GetMeetingReportBySessions =>
            BaseUrl + "/reports/meetings/{meetingId}/by-sessions"; //get

        public string DownloadMeetingReportBySessions =>
            BaseUrl + "/reports/meetings/{meetingId}/by-sessions/download"; //get

        public string DownloadMeetingDetailsReport =>
            BaseUrl + "/reports/meetings/{meetingId}/details/{meetingSessionId}/download"; //get

        public string GetRecordings => BaseUrl + "/meetings/{meetingId}/recordings"; //get
        public string GetTrashRecordings => BaseUrl + "/meetings/{meetingId}/recordings/trash"; //get

        public string DeleteRecordingFile =>
            BaseUrl + "/meetings/{meetingId}/recordings/files/{recordingFileId}"; //delete

        public string DeleteRecording =>
            BaseUrl + "/meetings/{meetingId}/recordings/{recordingId}"; //delete

        public string RecoverRecordingFile =>
            BaseUrl + "/meetings/{meetingId}/recordings/files/{recordingFileId}/status/recover"; //put

        public string RecoverRecording =>
            BaseUrl + "/meetings/{meetingId}/recordings/{recordingId}/status/recover"; //put

        public string GetTeacherAlailability => BaseUrl + "/office-hours/{meetingId}/availability"; //get
        public string AddTeacherAlailability => BaseUrl + "/office-hours/{meetingId}/availability"; //post
        public string GetSlots => BaseUrl + "/office-hours/{meetingId}/slots"; //get
        public string BookSlot => BaseUrl + "/office-hours/{meetingId}/slots"; //post
        public string CancelSlot => BaseUrl + "/office-hours/slots/{slotId}"; //delete
        public string DenySlot => BaseUrl + "/office-hours/slots/{slotId}/status/{status}"; //put

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
    }

}
