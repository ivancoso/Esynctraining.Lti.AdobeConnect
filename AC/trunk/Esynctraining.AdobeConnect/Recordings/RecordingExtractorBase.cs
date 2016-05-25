using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Recordings
{
    public abstract class RecordingExtractorBase
    {
        protected IAdobeConnectProxy AcProxy { get; private set; }


        protected RecordingExtractorBase(IAdobeConnectProxy acProxy)
        {
            AcProxy = acProxy;
        }
        

        public abstract IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone);

        public abstract IEnumerable<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone,
            int skip, int take);

        protected bool IsPublicRecording(string recordingScoId)
        {
            var moreDetails = AcProxy.GetScoPublicAccessPermissions(recordingScoId);
            var isPublic = false;
            if (moreDetails.Success && moreDetails.Values.Any())
            {
                isPublic = moreDetails.Values.First().PermissionId == PermissionId.view;
            }

            return isPublic;
        }

    }

}
