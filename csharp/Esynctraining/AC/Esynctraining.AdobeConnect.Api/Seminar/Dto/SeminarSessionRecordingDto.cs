using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.MeetingRecording.Dto;

namespace Esynctraining.AdobeConnect.Api.Seminar.Dto
{
    [DataContract]
    public class SeminarSessionRecordingDto : RecordingDto, ISeminarSessionRecordingDto
    {
        [DataMember]
        public string SeminarSessionId { get; set; }

        [DataMember]
        public string SeminarSessionName { get; set; }


        public SeminarSessionRecordingDto(Recording recording, string accountUrl, TimeZoneInfo timeZone)
            : base(recording, accountUrl, timeZone)
        {
        }
        
    }

}