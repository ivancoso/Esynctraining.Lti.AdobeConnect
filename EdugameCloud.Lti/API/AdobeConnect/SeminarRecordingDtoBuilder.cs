﻿using System;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class SeminarRecordingDtoBuilder : IRecordingDtoBuilder
    {
        public IRecordingDto Build(Recording recording, string accountUrl, TimeZoneInfo timeZone)
        {
            return new SeminarSessionRecordingDto(recording, accountUrl, timeZone);
        }

    }

}
