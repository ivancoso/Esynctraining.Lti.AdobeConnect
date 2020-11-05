using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Api.MeetingReports.Dto
{
    [DataContract]
    public class ACSessionParticipantDto : IComparable<ACSessionParticipantDto>
    {
        public ACSessionParticipantDto(MeetingAttendee source, TimeZoneInfo timeZone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            participantName = string.IsNullOrEmpty(source.ParticipantName) ? source.SessionName : source.ParticipantName; //guests have only sessionName
            sessionName = source.SessionName;
            login = source.Login;
            dateTimeEntered = FixACValue(source.DateCreated, timeZone).Value;
            dateTimeLeft = FixACValue(source.DateEnd, timeZone);
            durationInHours = (float)source.Duration.TotalHours;
            transcriptId = source.TranscriptId;
            
            scoId = source.ScoId;
            scoName = source.ScoName;
            assetId = source.AssetId;
            principalId = source.PrincipalId;
        }

        #region Public Properties

        [IgnoreDataMember]
        public string scoId { get; set; }

        [Required]
        [DataMember]
        public string participantName { get; set; }

        [IgnoreDataMember]
        public string scoName { get; set; }

        [IgnoreDataMember]
        public string sessionName { get; set; }

        /// <summary>
        /// Attendee's principal-id from AC.
        /// NOTE: principalId used by External API calls only.
        /// </summary>
        [Required]
        [DataMember(Name = "acId")]
        public string principalId { get; set; }

        [IgnoreDataMember]
        public string assetId { get; set; }

        [Required]
        [DataMember(Name = "enteredAt")]
        public DateTime dateTimeEntered { get; set; }

        [DataMember(Name = "leftAt")]
        public DateTime? dateTimeLeft { get; set; }

        [IgnoreDataMember]
        public float durationInHours { get; set; }

        /// <summary>
        /// Attendee's login(username) from AC.
        /// NOTE: login used by External API calls only.
        /// </summary>
        [Required]
        [DataMember]
        public string login { get; set; }

        [IgnoreDataMember]
        public string loginOrFullName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.login) ? this.login : this.participantName;
            }
        }

        [IgnoreDataMember]
        public string transcriptId { get; set; }

        #endregion

        public int CompareTo(ACSessionParticipantDto other)
        {
            return string.Compare(this.loginOrFullName, other.loginOrFullName, StringComparison.Ordinal);
        }


        private static DateTime? FixACValue(DateTime? dt, TimeZoneInfo timeZone)
        {
            if (dt.HasValue)
            {
                return FixACValue(dt.Value, timeZone);
            }
            return null;
        }

        private static DateTime? FixACValue(DateTime dt, TimeZoneInfo timeZone)
        {
            var tmp = dt < dt1951 ? (DateTime?)null : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);

            if (tmp.HasValue)
            {
                return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(tmp.Value, timeZone), DateTimeKind.Utc);
            }
            return null;
        }

        private static readonly DateTime dt1951 = new DateTime(1951, 1, 1);

    }

}
