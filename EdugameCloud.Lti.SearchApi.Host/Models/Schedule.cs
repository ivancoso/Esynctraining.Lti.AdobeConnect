using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int Interval { get; set; }
        public DateTime NextRun { get; set; }
        public int ScheduleDescriptor { get; set; }
        public int ScheduleType { get; set; }
        public bool IsEnabled { get; set; }
    }
}
