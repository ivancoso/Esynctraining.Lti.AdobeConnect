using System;
using System.Linq;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Recordings
{
    public abstract class RecordingExtractorBase
    {
        protected IAdobeConnectProxy AcProxy { get; }


        protected RecordingExtractorBase(IAdobeConnectProxy acProxy)
        {
            AcProxy = acProxy;
        }


        public abstract PagedResult<IRecordingDto> GetRecordings(IRecordingDtoBuilder dtoBuilder, string scoId, string accountUrl, TimeZoneInfo timeZone,
            string sortBy, string sortOrder, string search, long? dateFrom, long? dateTo, int skip, int take);

        protected bool IsPublicRecording(string recordingScoId)
        {
            var moreDetails = AcProxy.GetScoPublicAccessPermissions(recordingScoId, skipAcError: true);
            var isPublic = false;
            if (moreDetails.Success && moreDetails.Values.Any())
            {
                isPublic = moreDetails.Values.First().PermissionId == PermissionId.view;
            }

            return isPublic;
        }

    }

}
