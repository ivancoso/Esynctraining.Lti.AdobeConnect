using System;
using System.Collections;
using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class CreateMeetingViewModel
    {
        public string Topic { get; set; }

        public int? Type { get; set; }

        public long? StartTime { get; set; }

        public int? Duration { get; set; }

        public string Timezone { get; set; }

        public string Password { get; set; }

        public string Agenda { get; set; }

        public CreateMeetingSettingsViewModel Settings { get; set; }
        public CreateMeetingRecurrenceViewModel Recurrence { get; set; }
        public IEnumerable<RegistrantDto> Participants { get; set; }
    }
}