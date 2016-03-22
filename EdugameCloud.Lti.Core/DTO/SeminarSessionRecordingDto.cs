using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class SeminarSessionRecordingDto : RecordingDTO
    {
        public SeminarSessionRecordingDto(Recording recording, string accountUrl, TimeZoneInfo timeZone) : base(recording, accountUrl, timeZone)
        {
        }

        [DataMember(Name = "seminarSessionId")]
        public string seminarSessionId { get; set; }

        [DataMember(Name = "seminarSessionName")]
        public string seminarSessionName { get; set; }

    }

}
