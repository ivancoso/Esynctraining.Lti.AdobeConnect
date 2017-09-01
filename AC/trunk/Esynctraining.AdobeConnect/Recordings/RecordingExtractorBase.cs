using System;
using System.Collections.Generic;

namespace Esynctraining.AdobeConnect.Recordings
{
    public abstract class RecordingExtractorBase
    {
        protected IAdobeConnectProxy AcProxy { get; }


        protected RecordingExtractorBase(IAdobeConnectProxy acProxy)
        {
            AcProxy = acProxy;
        }


        public abstract IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone);
    }

}
