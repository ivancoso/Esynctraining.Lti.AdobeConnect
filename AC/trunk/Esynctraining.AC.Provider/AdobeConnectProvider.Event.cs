using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Extensions;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
//        public EventCollectionResult ReportMyEvents(int startIndex = 0, int limit = 0)
//        {
//            // act: "report-my-events"
//            StatusInfo status;
//
//            var doc = this.requestProcessor.Process(Commands.ReportMyEvents,
//                string.Empty.AppendPagingIfNeeded(startIndex, limit).TrimStart('&'), out status);
//
//            return ResponseIsOk(doc, status)
//                ? new EventCollectionResult(status, EventInfoCollectionParser.Parse(doc))
//                : new EventCollectionResult(status);
//        }

        //public void GetEventDynamicQuestionAnswers()
        //{
            
        //}

        //public void GetEventInfo()
        //{
            
        //}

        public async Task<StatusInfo> CreateEvent(SaveEventFields saveEventFields)
        {
            //Login as in UI, get 2 cookies and owasp and use it
            var loginResult = requestProcessor.LoginAsOnUi(saveEventFields.AdminUser);
            var owasp = loginResult.Owasp;

            var shortcutStatus = new StatusInfo();
            var eventsScoId = GetShortcutByType(ScoShortcutType.events.ToString(), out shortcutStatus).ScoId;
            var getResult = await requestProcessor.GetAcAdminResponseRedirectLocation(eventsScoId, owasp);
            
            var dict = AcCreateEventHelper.GetPostFormFields(saveEventFields, owasp);
            
            var result = requestProcessor.PostAcAdminRequest(new CreatingEventContainer()
            {
                EventProperties = dict,
                EventScoId = getResult.ScoId,
                SharedEventsFolderScoId = eventsScoId,
                Owasp = owasp,
                PostUrl = getResult.CreateEventPostUrl
            });
            
            return shortcutStatus;
        }


        public GenericResult<EventRegistrationDetails> GetEventRegistrationDetails(string scoId)
        {
            // act: "report-my-events"
            StatusInfo status;

            var doc = this.requestProcessor.Process("event-registration-details", $"sco-id={scoId}", out status);

            return ResponseIsOk(doc, status)
                ? new GenericResult<EventRegistrationDetails>(status, EventRegistrationDetailsParser.Parse(doc.SelectSingleNode("//results")))
                : new GenericResult<EventRegistrationDetails>(status, null);
        }

        public StatusInfo RegisterToEvent(EventRegistrationFormFields form)
        {
            // act: "event-register"
            StatusInfo status;

            var requestString = $"sco-id={form.ScoId}&login={UrlEncode(form.Email)}&password={form.Password}&password-verify={form.VerifyPassword}&first-name={form.FirstName}&last-name={form.LastName}";
            if (form.AdditionalFields.Values.Any())
            {
                foreach (var key in form.AdditionalFields.Keys)
                {
                    requestString += $"&interaction-id={key}&response={form.AdditionalFields[key]}";
                }
            }

            var doc = this.requestProcessor.Process("event-register", requestString, out status);

            return status;
        }
    }
}