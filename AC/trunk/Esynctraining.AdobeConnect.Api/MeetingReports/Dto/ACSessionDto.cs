using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Api.MeetingReports.Dto
{
    [DataContract]
    public class ACSessionDto
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionDto"/> class.
        /// </summary>
        public ACSessionDto()
        {
            this.participants = new List<ACSessionParticipantDto>();
        }

        public ACSessionDto(MeetingSession source, TimeZoneInfo timeZone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.participants = new List<ACSessionParticipantDto>();

            scoId = source.ScoId;
            assetId = source.AssetId;
            dateStarted = FixACValue(source.DateCreated, timeZone);
            dateEnded = FixACValue(source.DateEnd, timeZone);
            sessionNumber = int.Parse(source.Version);
            //sessionName = source.Version;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public List<ACSessionParticipantDto> participants { get; set; }

        [DataMember(Name = "startedAt")]
        public DateTime? dateStarted { get; set; }

        [DataMember(Name = "endedAt")]
        public DateTime? dateEnded { get; set; }
        
        // SSRS reports
        [IgnoreDataMember]
        public string scoId { get; set; }

        //public int acSessionId { get; set; }

        //public string meetingName { get; set; }

        //public string sessionName { get; set; }

        [DataMember]
        public int sessionNumber { get; set; }

        // SSRSReports
        [IgnoreDataMember]
        public string assetId { get; set; }

        #endregion

        private DateTime? FixACValue(DateTime? dt, TimeZoneInfo timeZone)
        {
            if (dt.HasValue)
            {
                return FixACValue(dt.Value, timeZone);
            }
            return null;
        }

        private DateTime? FixACValue(DateTime dt, TimeZoneInfo timeZone)
        {
            var tmp = dt < dt1951 ? (DateTime?)null : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);

            if (tmp.HasValue)
            {
                return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(tmp.Value, timeZone), DateTimeKind.Utc);
            }
            return null;
        }

        private readonly DateTime dt1951 = new DateTime(1951, 1, 1);
    }

}
