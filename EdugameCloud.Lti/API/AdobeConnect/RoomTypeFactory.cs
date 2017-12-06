using System;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api.MeetingRecording;
using Esynctraining.AdobeConnect.Recordings;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class RoomTypeFactory : IRoomTypeFactory
    {
        private readonly LmsMeetingType roomType;
        private readonly Esynctraining.AdobeConnect.IAdobeConnectProxy provider;
        private readonly ISeminarService seminarService;


        public RoomTypeFactory(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsMeetingType roomType, ISeminarService seminarService)
        {
            // TODO: null check

            this.provider = provider;
            this.roomType = roomType;
            this.seminarService = seminarService;
        }


        public RecordingExtractorBase GetRecordingExtractor()
        {
            switch (roomType)
            {
                case LmsMeetingType.Seminar:
                    return new SeminarRecordingExtractor(provider, seminarService);
                default:
                    return new RecordingExtractor(provider);
            }
        }
        
        public IRecordingDtoBuilder GetRecordingDtoBuilder()
        {
            switch (roomType)
            {
                case LmsMeetingType.Seminar:
                    return new SeminarRecordingDtoBuilder(false);
                default:
                    return new RecordingDtoBuilder(false);
            }
        }

    }

}
