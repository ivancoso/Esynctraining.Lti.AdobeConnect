using System;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class MeetingViewModel
    {
        public int Id { get; set; }
        public string ConferenceId { get; set; }
        //public string HostId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public int Duration { get; set; } //minutes
        public string Timezone { get; set; }

        public bool CanEdit { get; set; }
        public bool CanJoin { get; set; }
        public bool HasSessions { get; set; }

        public int Type { get; set; } //1 - meeting, 2 - office hours
        public string CourseId { get; set; }
    }

    public class OfficeHoursViewModel : MeetingViewModel
    {
        public MeetingDetailsViewModel Details { get; set; }
    }

    public class CreateMeetingViewModel
    {
        public string Topic { get; set; }

        public int? Type { get; set; }

        public DateTime StartTime { get; set; }

        public int Duration { get; set; }

        public string Timezone { get; set; }

        public string Password { get; set; }

        public string Agenda { get; set; }

        public CreateMeetingSettingsViewModel Settings { get; set; }
        public CreateMeetingRecurrenceViewModel Recurrence { get; set; }

    }

    public class MeetingDetailsViewModel : CreateMeetingViewModel
    {
        public int Id { get; set; }
        public string ConferenceId { get; set; }
        public string JoinUrl { get; set; }
    }
    public class CreateMeetingRecurrenceViewModel
    {
        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        public int Weeks { get; set; }

    }

    public class CreateMeetingSettingsViewModel
    {
        public int? RecurrenceRegistrationType { get; set; }

        public bool EnableHostVideo { get; set; }
        public bool EnableParticipantVideo { get; set; }
        public int AudioType { get; set; }
        public bool EnableJoinBeforeHost { get; set; }
        public bool EnableMuteOnEntry { get; set; }
        public bool EnableWaitingRoom { get; set; }
        public int? ApprovalType { get; set; }

        public int? RecordingType { get; set; }
        public string AlternativeHosts { get; set; }
    }
}