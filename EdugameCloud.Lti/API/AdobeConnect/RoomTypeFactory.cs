using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class RoomTypeFactory : IRoomTypeFactory
    {
        private readonly LmsMeetingType roomType;
        private readonly Esynctraining.AdobeConnect.IAdobeConnectProxy provider;
        private readonly ISeminarService seminarService;

        public RoomTypeFactory(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsMeetingType roomType, ISeminarService seminarService)
        {
            this.provider = provider;
            this.roomType = roomType;
            this.seminarService = seminarService;
        }

        public RecordingsExtractorBase GetRecordingsExtractor()
        {
            switch (roomType)
            {
                case LmsMeetingType.Seminar:
                    return new SeminarRecordingsExtractor(provider, seminarService);
                default:
                    return new RecordingsExtractor(provider);
            }
        }
    }
}
