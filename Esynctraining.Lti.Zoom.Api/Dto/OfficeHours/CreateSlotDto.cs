﻿using System;

namespace Esynctraining.Lti.Zoom.Api.Dto.OfficeHours
{
    public class CreateSlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Subject { get; set; }
        public string Questions { get; set; }
    }
}