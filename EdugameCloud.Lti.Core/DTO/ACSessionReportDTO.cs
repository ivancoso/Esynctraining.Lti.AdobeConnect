using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.Core.DTO
{
    public class ACSessionReportDTO
    {
        #region Constructors and Destructors

        public ACSessionReportDTO(ACSessionDTO acSession, int timezoneOffset)
        {
            if (acSession.participants == null || !acSession.participants.Any())
            {
                this.participants = new List<ACSessionParticipantReportDTO>();
            }
            else
            {
                this.participants = acSession.participants.Select(x => new ACSessionParticipantReportDTO(x, timezoneOffset)).ToList();
            }

            this.dateStarted = acSession.dateStarted == null ? null : (DateTime?)acSession.dateStarted.Value.ConvertToClientTime(timezoneOffset);
            this.dateEnded = acSession.dateEnded == null ? null : (DateTime?)acSession.dateEnded.Value.ConvertToClientTime(timezoneOffset);
            this.scoId = acSession.scoId;
            this.acSessionId = acSession.acSessionId;
            this.meetingName = acSession.meetingName;
            this.sessionName = acSession.sessionName;
            this.sessionNumber = acSession.sessionNumber;
            this.assetId = acSession.assetId;
        }

        #endregion

        #region Public Properties

        public List<ACSessionParticipantReportDTO> participants { get; set; }

        public DateTime? dateStarted { get; set; }

        public DateTime? dateEnded { get; set; }

        public int scoId { get; set; }

        public int acSessionId { get; set; }

        public string meetingName { get; set; }

        public string sessionName { get; set; }

        public int sessionNumber { get; set; }

        public int assetId { get; set; }

        public int participantsNumber
        {
            get
            {
                if (this.participants == null || !this.participants.Any())
                {
                    return 0;
                }

                return this.participants.Count;
            }
        }

        #endregion
    }
}
