using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public abstract class RecordingsExtractorBase
    {
        protected Esynctraining.AdobeConnect.IAdobeConnectProxy AcProxy { get; private set; }


        protected RecordingsExtractorBase(Esynctraining.AdobeConnect.IAdobeConnectProxy acProxy)
        {
            AcProxy = acProxy;
        }


        public abstract List<RecordingDTO> GetRecordings(string scoId, string accountUrl, TimeZoneInfo timeZone);


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
