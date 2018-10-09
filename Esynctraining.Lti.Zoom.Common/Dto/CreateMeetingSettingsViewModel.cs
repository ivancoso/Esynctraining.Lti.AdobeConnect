namespace Esynctraining.Lti.Zoom.Common.Dto
{
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