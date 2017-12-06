using System;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.MeetingRecording.Dto;

namespace Esynctraining.AdobeConnect.Api.MeetingRecording
{
    public class RecordingDtoBuilder : IRecordingDtoBuilder
    {
        private readonly bool _returnInitialRecordingDuration;


        public RecordingDtoBuilder()
        {
            // TRICK: to leave old code running as before
            _returnInitialRecordingDuration = true;
        }

        public RecordingDtoBuilder(bool returnInitialRecordingDuration)
        {
            _returnInitialRecordingDuration = returnInitialRecordingDuration;
        }


        public IRecordingDto Build(Recording recording, string accountUrl, TimeZoneInfo timeZone)
        {
            var item = new RecordingDto(recording, accountUrl, timeZone);
            if (_returnInitialRecordingDuration)
                return item;

            var t = recording.GetActualDuration();
            item.Duration = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
            return item;
        }

    }

}