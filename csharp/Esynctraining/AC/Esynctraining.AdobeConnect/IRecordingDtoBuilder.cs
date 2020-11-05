using System;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IRecordingDtoBuilder
    {
        IRecordingDto Build(Recording recording, string accountUrl, TimeZoneInfo timeZone);

    }

}
