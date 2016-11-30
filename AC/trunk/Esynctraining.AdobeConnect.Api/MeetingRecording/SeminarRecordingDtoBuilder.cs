using System;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;

namespace Esynctraining.AdobeConnect.Api.MeetingRecording
{
    public class SeminarRecordingDtoBuilder : IRecordingDtoBuilder
    {
        public IRecordingDto Build(Recording recording, string accountUrl, TimeZoneInfo timeZone)
        {
            return new SeminarSessionRecordingDto(recording, accountUrl, timeZone);
        }

    }
}