using System;
using System.Collections.Generic;
using System.Linq;

namespace Esynctraining.AdobeConnect.Api.MeetingReports.Dto
{
    public class ACSessionReportDto
    {
        #region Constructors and Destructors

        public ACSessionReportDto(ACSessionDto acSession, TimeZoneInfo timezone)
        {
            if (acSession.participants == null || !acSession.participants.Any())
            {
                this.participants = new List<ACSessionParticipantReportDto>();
            }
            else
            {
                this.participants = acSession.participants.Select(x => new ACSessionParticipantReportDto(x, timezone)).ToList();
            }

            this.dateStarted = acSession.dateStarted == null ? null : (DateTime?)ConvertToClientTime(acSession.dateStarted.Value, timezone);
            this.dateEnded = acSession.dateEnded == null ? null : (DateTime?)ConvertToClientTime(acSession.dateEnded.Value, timezone);
            this.scoId = acSession.scoId;
            //this.acSessionId = acSession.acSessionId;
            //this.meetingName = acSession.meetingName;
            //this.sessionName = acSession.sessionName;
            this.sessionNumber = acSession.sessionNumber;
            this.assetId = acSession.assetId;
        }

        #endregion

        #region Public Properties

        public List<ACSessionParticipantReportDto> participants { get; set; }

        public DateTime? dateStarted { get; set; }

        public DateTime? dateEnded { get; set; }

        public string scoId { get; set; }

        //public string acSessionId { get; set; }

        //public string meetingName { get; set; }

        //public string sessionName { get; set; }

        public int sessionNumber { get; set; }

        public string assetId { get; set; }

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

        private static DateTime ConvertToClientTime(DateTime date, TimeZoneInfo timeZone)
        {
            // TRICK: these information AC Provider returns in AC time zone - looks like that
            return date; // TimeZoneInfo.ConvertTimeFromUtc(date, timeZone);
        }

    }

}