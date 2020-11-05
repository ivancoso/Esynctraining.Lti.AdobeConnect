namespace eSyncTraining.Web.Models
{
    using System.Collections.Generic;

    public class MeetingParticipantsModel
    {
        public IEnumerable<PrincipalSlimModel> Principals { get; set; }

        public IEnumerable<ParticipantSlimModel> Hosts { get; set; }

        public IEnumerable<ParticipantSlimModel> Presenters { get; set; }

        public IEnumerable<ParticipantSlimModel> Participants { get; set; }
    }
}
