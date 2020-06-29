using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Telephony
{
    public interface ITelephonyProfileEngine
    {
        /// <summary>
        /// Returns null if profile wasn't created.
        /// </summary>
        TelephonyProfile CreateProfile(ILmsLicense lmsCompany, ILtiParam param, string profileName, IAdobeConnectProxy acProxy);

        void DeleteProfile(ILmsLicense lmsCompany, TelephonyProfileInfoResult profile);

    }

}
