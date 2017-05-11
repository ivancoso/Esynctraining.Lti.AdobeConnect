﻿using System;
using System.Linq;
using System.Net.Http;
using System.Resources;
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

        public async Task<SaveEventResponse> CreateEvent(SaveEventFields saveEventFields)
        {
            if (saveEventFields.AdminUser == null || string.IsNullOrEmpty(saveEventFields.AdminUser.Login) ||
                string.IsNullOrEmpty(saveEventFields.AdminUser.Password))
            {
                throw new InvalidOperationException(nameof(saveEventFields.AdminUser));
            }
            if (saveEventFields.StartDate == DateTime.MinValue || saveEventFields.StartDate == DateTime.MaxValue)
            {
                throw new InvalidOperationException(nameof(saveEventFields.StartDate));
            }
            if (string.IsNullOrEmpty(saveEventFields.Name))
            {
                throw new InvalidOperationException(nameof(saveEventFields.Name));
            }

            //Login as in UI, get 2 cookies and owasp and use it
            var loginResult = requestProcessor.LoginAsOnUi(saveEventFields.AdminUser);
            var owasp = loginResult.Owasp;

            StatusInfo status;
            var shortcutByType = GetShortcutByType(ScoShortcutType.events.ToString(), out status);
            var eventsScoId = shortcutByType.ScoId;

            var myMeetings = GetShortcutByType(ScoShortcutType.my_meetings.ToString(), out status);
            saveEventFields.ListScoId = myMeetings.ScoId;
            //saveEventFields.ListScoId = "304292";

            var getResult = await requestProcessor.GetAcAdminResponseRedirectLocation(eventsScoId, owasp);
            saveEventFields.EventTemplateId = getResult.EventTemplateId;
            var dict = AcCreateEventHelper.GetPostFormFields(saveEventFields, owasp);
            var commonInfo = GetCommonInfo().CommonInfo;
            var accountId = commonInfo.AccountId?.ToString() ??
            throw new InvalidOperationException("Can't get common info");
            

            var result = requestProcessor.PostAcAdminRequest(new CreatingEventContainer()
            {
                EventProperties = dict,
                EventScoId = getResult.ScoId,
                SharedEventsFolderScoId = eventsScoId,
                Owasp = owasp,
                PostUrl = getResult.CreateEventPostUrl,
                AccountId = accountId
            });

            return new SaveEventResponse()
            {
                EventScoId = getResult.ScoId,
                StartDate = saveEventFields.StartDate,
                EndDate = saveEventFields.EndDate,
                EventTitle = saveEventFields.Name
            };
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