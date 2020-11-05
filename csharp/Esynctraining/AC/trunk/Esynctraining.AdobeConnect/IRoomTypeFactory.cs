using Esynctraining.AdobeConnect.Recordings;

namespace Esynctraining.AdobeConnect
{
    public interface IRoomTypeFactory
    {
        RecordingExtractorBase GetRecordingExtractor();

        IRecordingDtoBuilder GetRecordingDtoBuilder();

    }
    
}
