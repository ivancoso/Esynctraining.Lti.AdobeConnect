using System;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.Seminar.Dto;

namespace Esynctraining.AdobeConnect.Api.MeetingRecording
{
    public class SeminarRecordingDtoBuilder : IRecordingDtoBuilder
    {
        private readonly bool _returnInitialRecordingDuration;


        public SeminarRecordingDtoBuilder()
        {
            // TRICK: to leave old code running as before
            _returnInitialRecordingDuration = true;
        }

        public SeminarRecordingDtoBuilder(bool returnInitialRecordingDuration)
        {
            _returnInitialRecordingDuration = returnInitialRecordingDuration;
        }


        public IRecordingDto Build(Recording recording, string accountUrl, TimeZoneInfo timeZone)
        {
            var item = new SeminarSessionRecordingDto(recording, accountUrl, timeZone);
            if (_returnInitialRecordingDuration)
                return item;

            var t = recording.GetActualDuration();
            item.Duration = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
            return item;
        }

    }

}