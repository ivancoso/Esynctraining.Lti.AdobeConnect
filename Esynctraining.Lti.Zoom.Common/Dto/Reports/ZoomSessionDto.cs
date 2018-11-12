using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.Lti.Zoom.Common.Dto.Reports
{
    [DataContract]
    public class ZoomSessionDto
    {
        public ZoomSessionDto()
        {
            this.Participants = new List<ZoomSessionParticipantDto>();
        }

        //public ACSessionDto(MeetingSession source, TimeZoneInfo timeZone)
        //{
        //    if (source == null)
        //        throw new ArgumentNullException(nameof(source));
        //    this.Participants = new List<ACSessionParticipantDto>();
        //    this.scoId = source.ScoId;
        //    this.assetId = source.AssetId;
        //    this.dateStarted = this.FixACValue(source.DateCreated, timeZone);
        //    this.dateEnded = this.FixACValue(source.DateEnd, timeZone);
        //    this.sessionNumber = int.Parse(source.Version);
        //}

        [DataMember]
        public List<ZoomSessionParticipantDto> Participants { get; set; }

        [DataMember(Name = "startedAt")]
        public DateTime StartedAt { get; set; }

        [DataMember(Name = "endedAt")]
        public DateTime EndedAt { get; set; }

        public int Duration { get; set; }

        //[IgnoreDataMember]
        //[DataMember]
        //public string id { get; set; }

        [DataMember]
        public string SessionId { get; set; }

    }
}