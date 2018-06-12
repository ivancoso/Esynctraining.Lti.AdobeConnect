using System;
using System.Runtime.Serialization;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    [DataContract]
    public class CreateZoomMeetingDto
    {
        [DataMember]
        public string Topic { get; set; }

        public ZoomMeetingType Type { get; set; }

        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string Timezone { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Agenda { get; set; }

        [DataMember]
        public CreateMeetingSettingsDto Settings { get; set; }
        [DataMember]
        public CreateMeetingRecurrenceDto Recurrence { get; set; }

    }
}