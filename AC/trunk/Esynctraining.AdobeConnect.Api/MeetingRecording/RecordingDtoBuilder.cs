using System;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.MeetingRecording.Dto;

namespace Esynctraining.AdobeConnect.Api.MeetingRecording
{
    public class RecordingDtoBuilder : IRecordingDtoBuilder
    {
        public IRecordingDto Build(Recording recording, string accountUrl, TimeZoneInfo timeZone)
        {
            return new RecordingDto(recording, accountUrl, timeZone);
        }

    }
}