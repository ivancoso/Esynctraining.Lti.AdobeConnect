using System;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        //public UserCollectionResult ReportGuests()
        //{
        //    // act: "report-bulk-users"
        //    StatusInfo status;

        //    var doc = this.requestProcessor.Process(Commands.ReportBulkUsers, CommandParams.ReportBulkUsersFilters.Guest, out status);

        //    return ResponseIsOk(doc, status)
        //               ? new UserCollectionResult(status, UserCollectionParser.Parse(doc, this.requestProcessor.ServiceUrl, null))
        //               : new UserCollectionResult(status);
        //}

        public UserCollectionResult ReportGuestsByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login filter value can not be empty", nameof(login));

            // act: "report-bulk-users"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkUsers,
                string.Format(CommandParams.ReportBulkUsersFilters.GuestByLogin, UrlEncode(login)), out status);

            return ResponseIsOk(doc, status)
                       ? new UserCollectionResult(status, UserCollectionParser.Parse(doc))
                       : new UserCollectionResult(status);
        }

        public UserCollectionResult ReportGuestsByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email filter value can not be empty", nameof(email));

            // act: "report-bulk-users"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.ReportBulkUsers,
                string.Format(CommandParams.ReportBulkUsersFilters.GuestByEmail, UrlEncode(email)), out status);

            return ResponseIsOk(doc, status)
                       ? new UserCollectionResult(status, UserCollectionParser.Parse(doc))
                       : new UserCollectionResult(status);
        }

    }

}
