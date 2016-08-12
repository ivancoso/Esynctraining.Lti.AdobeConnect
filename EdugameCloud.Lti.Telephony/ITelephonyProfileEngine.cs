using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.Telephony
{
    public interface ITelephonyProfileEngine
    {
        /// <summary>
        /// Returns null if profile wasn't created.
        /// </summary>
        TelephonyProfile CreateProfile(LmsCompany lmsCompany, LtiParamDTO param, string profileName, IAdobeConnectProxy acProxy);

        void DeleteProfile(LmsCompany lmsCompany, TelephonyProfileInfoResult profile);

    }

}
