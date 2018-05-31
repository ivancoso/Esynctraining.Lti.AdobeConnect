﻿using System.Runtime.Serialization;
using RestSharp.Deserializers;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    [DataContract]
    public class MeetingSettings
    {
        [DeserializeAs(Name = "host_video")]
        public bool EnableHostVideo { get; set; }

        [DeserializeAs(Name = "participant_video")]
        public bool EnableParticipantVideo { get; set; }

        [DeserializeAs(Name = "cn_meeting")]
        public bool EnableChinaHost { get; set; }

        [DeserializeAs(Name = "in_meeting")]
        public bool EnableIndiaHost { get; set; }

        [DeserializeAs(Name = "join_before_host")]
        public bool EnableJoinBeforeHost { get; set; }

        [DeserializeAs(Name = "mute_upon_entry")]
        public bool EnableMuteOnEntry { get; set; }

        [DeserializeAs(Name = "watermark")]
        public bool EnableWatermark { get; set; }

        [DeserializeAs(Name = "use_pmi")]
        public bool UsePersonalMeetingId { get; set; }

        [DataMember]
        public MeetingApprovalTypes ApprovalType { get; set; }

        [DataMember]
        public MeetingRegistrationTypes? RegistrationType { get; set; }

        [DataMember]
        public string Audio { get; set; }

        [DataMember]
        public string AutoRecording { get; set; }

        [DeserializeAs(Name = "enforce_login")]
        public bool EnableEnforceLogin { get; set; }

        [DeserializeAs(Name = "enforce_login_domains")]
        public string EnableEnforceLoginDomains { get; set; }

        public bool RegistrantsConfirmationEmail { get; set; }

        [DataMember]
        public string AlternativeHosts { get; set; }
    }
}