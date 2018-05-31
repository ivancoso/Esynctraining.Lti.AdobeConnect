﻿using System.Runtime.Serialization;
using Edugamecloud.Lti.Zoom.Dto.Enums;

namespace Edugamecloud.Lti.Zoom.Dto
{
    [DataContract]
    public class CreateMeetingSettingsDto
    {
        [DataMember]
        public MeetingRegistrationTypes? RegistrationType { get; set; }

        [DataMember]
        public ApprovalTypes ApprovalType { get; set; }

        [DataMember]
        public bool EnableHostVideo { get; set; }
        [DataMember]
        public bool EnableParticipantVideo { get; set; }
        [DataMember]
        public MeetingAudioType AudioType { get; set; }
        [DataMember]
        public bool EnableJoinBeforeHost { get; set; }
        [DataMember]
        public bool EnableMuteOnEntry { get; set; }
        [DataMember]
        public bool EnableWaitingRoom { get; set; }
        [DataMember]
        public AutomaticRecordingType RecordingType { get; set; }
        [DataMember]
        public string AlternativeHosts { get; set; }
    }
}