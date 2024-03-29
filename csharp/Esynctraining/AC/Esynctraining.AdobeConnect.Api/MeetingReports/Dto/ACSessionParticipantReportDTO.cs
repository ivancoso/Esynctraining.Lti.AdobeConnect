﻿using System;

namespace Esynctraining.AdobeConnect.Api.MeetingReports.Dto
{
    public class ACSessionParticipantReportDto
    {
        #region Constructors and Destructors

        public ACSessionParticipantReportDto(ACSessionParticipantDto acSession, TimeZoneInfo timezone)
        {
            if (acSession == null)
                throw new ArgumentNullException(nameof(acSession));
            if (timezone == null)
                throw new ArgumentNullException(nameof(timezone));

            this.scoId = acSession.scoId;
            participantName = string.IsNullOrEmpty(acSession.participantName)
                ? acSession.sessionName
                : acSession.participantName; //guests have only sessionName
            this.scoName = acSession.scoName;
            this.sessionName = acSession.sessionName;
            this.principalId = acSession.principalId;
            this.assetId = acSession.assetId;
            this.dateTimeEntered = ConvertToClientTime(acSession.dateTimeEntered, timezone);
            this.dateTimeLeft = acSession.dateTimeLeft == null ? null : (DateTime?)ConvertToClientTime(acSession.dateTimeLeft.Value, timezone);
            this.duration = GetParticipantAttendanceDuration(acSession.durationInHours);
            this.login = acSession.login;
            //this.firstName = acSession.firstName;
            //this.lastName = acSession.lastName;
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

        //public string loginOrFullName
        //{
        //    get
        //    {
        //        return !string.IsNullOrWhiteSpace(this.login) ? this.login : this.fullName;
        //    }
        //}

        //public string fullName
        //{
        //    get
        //    {
        //        return string.IsNullOrWhiteSpace(this.lastName) ? this.firstName : string.Format("{0} {1}", this.firstName, this.lastName);
        //    }
        //}

        //public string firstName { get; set; }
        //public string lastName { get; set; }
        public string transcriptId { get; set; }

        #endregion

        #region methods

        private static string GetParticipantAttendanceDuration(float duration)
        {
            var timeStamp = TimeSpan.FromHours(duration);
            var totalSeconds = (int)timeStamp.TotalSeconds;
            return TimeSpan.FromSeconds(totalSeconds).ToString();
        }

        #endregion

        public static DateTime ConvertToClientTime(DateTime date, TimeZoneInfo timeZone)
        {
            // TRICK: these information AC Provider returns in AC time zone - looks like that
            return date;// TimeZoneInfo.ConvertTimeFromUtc(date, timeZone);
        }

    }

}
