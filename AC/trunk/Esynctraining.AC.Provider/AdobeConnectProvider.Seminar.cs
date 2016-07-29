using System;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
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

        public SeminarLicensesCollectionResult GetSeminarLicenses(string scoId, bool returnUserSeminars = false)
        {
            // act: "sco-seminar-licenses-list"
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Seminar.SeminarLicensesList, 
                string.Format("{0}{1}", 
                    string.Format(CommandParams.ScoId, scoId), 
                    returnUserSeminars ? "&user-webinar-selected=true" : string.Empty),
                out status);

            return ResponseIsOk(doc, status)
                       ? new SeminarLicensesCollectionResult(status, SeminarLicensesCollectionParser.Parse(
                           doc.SelectSingleNode(returnUserSeminars? UserSeminarLicensesHome : SharedSeminarLicensesHome)))
                       : new SeminarLicensesCollectionResult(status);
        }
    }

}
