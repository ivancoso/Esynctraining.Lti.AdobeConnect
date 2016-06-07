using System;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public partial class AdobeConnectProxy : IAdobeConnectProxy
    {
        public TelephonyProviderCollectionResult TelephonyProviderList(string principalId)
        {

            TelephonyProviderCollectionResult result;
            try
            {
                result = _provider.TelephonyProviderList(principalId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "TelephonyProviderList. PrincipalId:{0}", principalId);
                throw new AdobeConnectException("TelephonyProviderList exception", ex);
            }

            return result;
        }

        public TelephonyProfilesCollectionResult TelephonyProfileList(string principalId)
        {

            TelephonyProfilesCollectionResult result;
            try
            {
                result = _provider.TelephonyProfileList(principalId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "TelephonyProfileList. PrincipalId:{0}", principalId);
                throw new AdobeConnectException("TelephonyProfileList exception", ex);
            }

            return result;
        }

        public TelephonyProfileInfoResult TelephonyProfileInfo(string profileId)
        {
            TelephonyProfileInfoResult result;
            try
            {
                result = _provider.TelephonyProfileInfo(profileId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "TelephonyProfileInfo. ProfileId:{0}", profileId);
                throw;
            }

            return result;
        }

        public TelephonyProfileInfoResult TelephonyProfileUpdate(TelephonyProfileUpdateItem updateItem, bool isUpdate)
        {
            if (updateItem == null)
                throw new ArgumentNullException(nameof(updateItem));

            TelephonyProfileInfoResult result;
            try
            {
                result = _provider.TelephonyProfileUpdate(updateItem, isUpdate);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "TelephonyProfileUpdate. ProfileName:{0}", updateItem.ProfileName);
                throw;
            }

            return result;
        }

    }

}
