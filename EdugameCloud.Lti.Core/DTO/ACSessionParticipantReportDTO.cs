using System;
using Esynctraining.Core.Extensions;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Core.DTO
{
    public class ACSessionParticipantReportDTO
    {
        #region Constructors and Destructors

        public ACSessionParticipantReportDTO(ACSessionParticipantDTO acSession, int timezoneOffset)
        {
            this.scoId = acSession.scoId;
            this.participantName = acSession.participantName;
            this.scoName = acSession.scoName;
            this.sessionName = acSession.sessionName;
            this.principalId = acSession.principalId;
            this.assetId = acSession.assetId;
            this.dateTimeEntered = acSession.dateTimeEntered.ConvertToClientTime(timezoneOffset);
            this.dateTimeLeft = acSession.dateTimeLeft == null ? null: (DateTime?)acSession.dateTimeLeft.Value.ConvertToClientTime(timezoneOffset);
            this.duration = GetParticipantAttendanceDuration(acSession.durationInHours);
            this.login = acSession.login;
            this.firstName = acSession.firstName;
            this.lastName = acSession.lastName;
            this.transcriptId = acSession.transcriptId;

        }

        #endregion

        #region Public Properties
        public string scoId { get; set; }
        public string participantName { get; set; }
        public string scoName { get; set; }
        public string sessionName { get; set; }
        public string principalId { get; set; }
        public string assetId { get; set; }
        public DateTime dateTimeEntered { get; set; }
        public DateTime? dateTimeLeft { get; set; }
        public string duration { get; set; }
        public string login { get; set; }
        public string loginOrFullName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.login) ? this.login : this.fullName;
            }
        }
        public string fullName
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.lastName) ? this.firstName : string.Format("{0} {1}", this.firstName, this.lastName);
            }
        }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long transcriptId { get; set; }

        #endregion

        #region methods

        private static string GetParticipantAttendanceDuration(float duration)
        {
            var timeStamp = TimeSpan.FromHours(duration);
            var totalSeconds = (int)timeStamp.TotalSeconds;
            return TimeSpan.FromSeconds(totalSeconds).ToString();
        }

        #endregion
    }
}
