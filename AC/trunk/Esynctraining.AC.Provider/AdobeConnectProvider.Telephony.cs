using System;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider
{
    //http://blogs.adobe.com/connectsupport/xml-api-tips-creating-telephony-profiles-via-the-xml-api/

    // providers: https://connect.esynctraining.com/api/xml?action=telephony-provider-list
    // MeetingOne provider fields: https://connect.esynctraining.com/api/xml?action=telephony-provider-field-list&provider-id=21002
    public partial class AdobeConnectProvider
    {
        private static readonly string TelephonyProfileHome = "//telephony-profile";
        private static readonly string TelephonyProfileFieldsHome = "//telephony-profile-fields";


        /// <summary>
        /// Returns a list of telephony providers.
        /// </summary>
        /// <param name="principalId">if provided - returns a list of user-configured providers based on the value of principalId.
        /// If not provided - method returns a list of account-level providers.</param>
        /// <returns></returns>
        public TelephonyProviderCollectionResult TelephonyProviderList(string principalId)
        {
            // act: "telephony-provider-list"
            StatusInfo status;

            string parameters = string.IsNullOrWhiteSpace(principalId)
                ? string.Empty
                : string.Format(CommandParams.Telephony.UserConfiguredProfiles, principalId);

            var doc = this.requestProcessor.Process(Commands.Telephony.ProviderList, parameters, out status);

            string path = string.IsNullOrWhiteSpace(principalId) ? TelephonyProviderCollectionParser.AccountPath : TelephonyProviderCollectionParser.UserPath;

            return ResponseIsOk(doc, status)
                       ? new TelephonyProviderCollectionResult(status, TelephonyProviderCollectionParser.Parse(doc, path))
                       : new TelephonyProviderCollectionResult(status);
        }


        public TelephonyProfilesCollectionResult TelephonyProfileList(string principalId)
        {
            if (string.IsNullOrEmpty(principalId))
                throw new ArgumentNullException("Non-empty value expected", nameof(principalId));

            // act: "telephony-profile-list"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Telephony.ProfileList, string.Format(CommandParams.PrincipalId, principalId), out status);

            return ResponseIsOk(doc, status)
                       ? new TelephonyProfilesCollectionResult(status, TelephonyProfilesCollectionParser.Parse(doc))
                       : new TelephonyProfilesCollectionResult(status);
        }

        public TelephonyProfileInfoResult TelephonyProfileInfo(string profileId)
        {
            // act: "telephony-profile-info"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Telephony.ProfileInfo, string.Format(CommandParams.Telephony.ProfileId, profileId), out status);

            return ResponseIsOk(doc, status)
                       ? new TelephonyProfileInfoResult(status, TelephonyProfileParser.Parse(doc.SelectSingleNode(TelephonyProfileHome)), TelephonyProfileFieldsParser.Parse(doc.SelectSingleNode(TelephonyProfileFieldsHome)))
                       : new TelephonyProfileInfoResult(status);
        }

        public TelephonyProfileInfoResult TelephonyProfileUpdate(TelephonyProfileUpdateItem updateItem, bool isUpdate)
        {
            // act: "telephony-profile-update"
            if (updateItem == null)
                throw new ArgumentNullException(nameof(updateItem));

            var commandParams = QueryStringBuilder.EntityToQueryString(updateItem);
            if (updateItem.ProviderFields != null)
                commandParams += updateItem.ProviderFields.ToQueryString();

            StatusInfo status;
            var doc = this.requestProcessor.Process(Commands.Telephony.ProfileUpdate, commandParams, out status);

            if (!ResponseIsOk(doc, status))
            {
                return new TelephonyProfileInfoResult(status);
            }

            if (isUpdate)
            {
                return this.TelephonyProfileInfo(updateItem.ProfileId);
            }

            // notice: no 'profile' will be returned during update!!
            //https://helpx.adobe.com/adobe-connect/webservices/telephony-provider-update.html
            var detailNode = doc.SelectSingleNode("results/telephony-profile");
            if (detailNode == null || detailNode.Attributes == null)
            {
                return new TelephonyProfileInfoResult(status);
            }

            TelephonyProfile detail = null;

            try
            {
                detail = TelephonyProfileParser.Parse(detailNode);
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
                status.Code = StatusCodes.invalid;
                status.SubCode = StatusSubCodes.format;
                status.UnderlyingExceptionInfo = ex;
            }

            return new TelephonyProfileInfoResult(status, detail);
        }

    }

}
