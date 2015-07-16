using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.DTO;
using NHibernate.Cfg.MappingSchema;

namespace EdugameCloud.Lti.Core.DTO
{
    public class MeetingSessionReportDTO
    {

        public MeetingSessionReportDTO(ACSessionDTO session)
        {
            this.StartTime = session.dateStarted;
            this.EndTime = session.dateEnded;
            this.AttendeesNumber = session.participants.Count;
            this.ACSessionId = session.acSessionId;
        }

        public int ACSessionId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int AttendeesNumber{ get; set; }

    }
}
