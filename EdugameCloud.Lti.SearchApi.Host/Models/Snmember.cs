using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class Snmember
    {
        public int SnMemberId { get; set; }
        public int AcSessionId { get; set; }
        public string Participant { get; set; }
        public string ParticipantProfile { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool IsBlocked { get; set; }

        public virtual Acsession AcSession { get; set; }
    }
}
