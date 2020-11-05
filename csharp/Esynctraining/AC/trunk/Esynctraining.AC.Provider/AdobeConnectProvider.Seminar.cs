using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        private const string SharedSeminarLicensesHome = "//seminar-licenses";
        private const string UserSeminarLicensesHome = "//user-webinar-licenses";

        public ScoInfoResult SeminarSessionScoUpdate(SeminarSessionScoUpdateItem item)
        {
            if (item == null)
            {
                return new ScoInfoResult(new StatusInfo { Code = StatusCodes.internal_error });
            }

            var commandParams = QueryStringBuilder.EntityToQueryString(item);

            StatusInfo status;
            var doc = this.requestProcessor.Process(Commands.Seminar.SeminarSessionScoUpdate, commandParams, out status);

            if (!ResponseIsOk(doc, status))
            {
                return new ScoInfoResult(status);
            }

            //if (isUpdate)
            {
                return this.GetScoInfo(item.ScoId);
            }

            // notice: no '/sco' will be returned during update
            //var detailNode = doc.SelectSingleNode(ScoHome);

            //if (detailNode == null || detailNode.Attributes == null)
            //{
            //    return new ScoInfoResult(status);
            //}

            //ScoInfo meetingDetail = null;

            //try
            //{
            //    meetingDetail = ScoInfoParser.Parse(detailNode);
            //}
            //catch (Exception ex)
            //{
            //    TraceTool.TraceException(ex);
            //    status.Code = StatusCodes.invalid;
            //    status.SubCode = StatusSubCodes.format;
            //    status.UnderlyingExceptionInfo = ex;

            //    // delete meeting
            //    // [DD]: why would you do that?!..
            //    // if (meetingDetail != null && !string.IsNullOrEmpty(meetingDetail.scoId))
            //    // {
            //    // this.DeleteSco(meetingDetail.scoId);
            //    // }
            //}

            //return new ScoInfoResult(status, meetingDetail);
        }

        public CollectionResult<SharedSeminarLicenseSco> GetSharedSeminarLicenses(string scoId)
        {
            // act: "sco-seminar-licenses-list"
            var args = string.Format(CommandParams.ScoId, scoId);

            return GetCollection(Commands.Seminar.SeminarLicensesList, args, SharedSeminarLicensesHome, "//sco",
                SharedSeminarLicenseScoParser.Parse);
        }

        public CollectionResult<UserSeminarLicenseSco> GetUserSeminarLicenses(string scoId)
        {
            // act: "sco-seminar-licenses-list"
            var args = $"{string.Format(CommandParams.ScoId, scoId)}&user-webinar-selected=true";

            return GetCollection(Commands.Seminar.SeminarLicensesList, args, UserSeminarLicensesHome, "//sco",
                UserSeminarLicenseScoParser.Parse);
        }
    }
}
