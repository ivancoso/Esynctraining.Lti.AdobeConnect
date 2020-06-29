using System;
using System.Runtime.Serialization;
//using AndcultureCode.ZoomClient.Models.Meetings;

namespace Esynctraining.Lti.Zoom.Api.Host.Models
{
    //public class MeetingViewModel
    //{
    //    public int Id { get; set; }
    //    public string ConferenceId { get; set; }
    //    //public string HostId { get; set; }
    //    public string Topic { get; set; }
    //    public string Description { get; set; }
    //    public DateTime? StartTime { get; set; }
    //    public int Duration { get; set; } //minutes
    //    public string Timezone { get; set; }

    //    public bool CanEdit { get; set; }
    //    public bool CanJoin { get; set; }
    //    public bool HasSessions { get; set; }

    //    public int Type { get; set; } //1 - meeting, 2 - office hours
    //    public string CourseId { get; set; }
    //    /*
    //"uuid": "BFOLw+u6T1mOB/dH9B/H/g==",
    //"id": 378204340,
    //"host_id": "hxYospYTROeo73rtvVq-OA",
    //"topic": "Test From Zoom UI",
    //"type": 8,
    //"start_time": "2018-05-23T11:00:00Z",
    //"duration": 60,
    //"timezone": "Europe/Moscow",
    //*/
    //}

    public class ExtendedReportParticipantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }
    /*
     * {
            "id": "9St6ertHQ_m5d9rIl319aQ",
            "name": "Mike Kollen",
            "device": "Mac",
            "ipAddress": "68.101.110.42",
            "location": "Laguna Beach (US)",
            "networkType": "Wifi",
            "startTime": long,
            "endTime": long,
            "shareApplication": false,
            "shareDesktop": false,
            "shareWhiteboard": false,
            "recording": false,
            "pcName": "",
            "domain": "",
            "macAddr": "",
            "harddiskId": "",
            "version": "4.1.23501.0416"
      }   
         */

    public enum ZoomMeetingType
    {
        Instant = 1,
        Scheduled = 2,
        RecurringNoTime = 3,
        RecurringWithTime = 4
    }

    [DataContract]
    public class CreateMeetingViewModel
    {
        [DataMember]
        public string Topic { get; set; }

        [DataMember]
        public int? Type { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string Timezone { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Agenda { get; set; }

        [DataMember]
        public CreateMeetingSettingsViewModel Settings { get; set; }
        [DataMember]
        public CreateMeetingRecurrenceViewModel Recurrence { get; set; }

    }

    //[DataContract]
    //public class MeetingDetailsViewModel : CreateMeetingViewModel
    //{
    //    [DataMember]
    //    public int Id { get; set; }
    //    [DataMember]
    //    public string ConferenceId { get; set; }
    //}

    [DataContract]
    public class CreateMeetingRecurrenceViewModel
    {
        // TRICK: to support JIL instead of DayOfWeek
        [DataMember]
        public int[] DaysOfWeek { get; set; }

        [DataMember]
        public int Weeks { get; set; }

    }

    [DataContract]
    public class CreateMeetingSettingsViewModel
    {
        [DataMember]
        public int? RecurrenceRegistrationType { get; set; }

        [DataMember]
        public bool EnableHostVideo { get; set; }
        [DataMember]
        public bool EnableParticipantVideo { get; set; }
        [DataMember]
        public int AudioType { get; set; }
        [DataMember]
        public bool EnableJoinBeforeHost { get; set; }
        [DataMember]
        public bool EnableMuteOnEntry { get; set; }
        [DataMember]
        public bool EnableWaitingRoom { get; set; }
        [DataMember]
        public int? ApprovalType { get; set; }

        [DataMember]
        public int? RecordingType { get; set; }
        [DataMember]
        public string AlternativeHosts { get; set; }
    }

    public enum MeetingRegistrationTypes
    {
        NoRegistration = 0,RegisterAllOccurrences = 1,RegisterEachOccurrence = 2,RegisterChooseOccurrence = 3,
    }

    public enum MeetingAudioType
    {
        Telephone = 1,Computer = 2,Both = 3
    }

    public enum AutomaticRecordingType
    {
        None = 0,Local = 1,Cloud = 2
    }
}
