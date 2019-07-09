using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class AppletResult
    {
        public int AppletResultId { get; set; }
        public int AppletItemId { get; set; }
        public int AcSessionId { get; set; }
        public string ParticipantName { get; set; }
        public int Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsArchive { get; set; }
        public string Email { get; set; }

        public virtual Acsession AcSession { get; set; }
        public virtual AppletItem AppletItem { get; set; }
    }
}
