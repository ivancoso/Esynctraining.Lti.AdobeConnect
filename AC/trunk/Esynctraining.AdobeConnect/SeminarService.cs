using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect
{
    public class SeminarService : ISeminarService
    {
        private static readonly string AcDateFormat = "yyyy-MM-ddTHH:mm"; // AdobeConnectProviderConstants.DateFormat

        private readonly ILogger _logger;


        public SeminarService(ILogger logger)
        {
            _logger = logger;
        }


        public IEnumerable<ScoContent> GetAllSeminarLicenses(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            StatusInfo status;
            ScoShortcut seminarShortcut = provider.GetShortcutByType("seminars", out status);
            if (status.Code != StatusCodes.ok)
                throw new AdobeConnectException(status);
            if (seminarShortcut == null)
                throw new InvalidOperationException("Seminars are not acessible for the principal");

            var result = provider.GetContentsByScoId(seminarShortcut.ScoId);
            return result.Values;
        }

        public IEnumerable<SharedSeminarLicenseSco> GetSharedSeminarLicenses(IAdobeConnectProxy provider)
        {
            var seminarsShortcutId = GetSeminarsShortcutId(provider);
            var result = provider.GetSharedSeminarLicenses(seminarsShortcutId);
            return result.Values;
        }

        public IEnumerable<UserSeminarLicenseSco> GetUserSeminarLicenses(IAdobeConnectProxy provider)
        {

            var seminarsShortcutId = GetSeminarsShortcutId(provider);
            var result = provider.GetUserSeminarLicenses(seminarsShortcutId);
            return result.Values;
        }

        private string GetSeminarsShortcutId(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            StatusInfo status;
            ScoShortcut seminarShortcut = provider.GetShortcutByType("seminars", out status);
            if (status.Code != StatusCodes.ok)
                throw new AdobeConnectException(status);

            if (seminarShortcut == null)
                throw new InvalidOperationException("Seminars are not acсessible for the principal");

            return seminarShortcut.ScoId;
        }

        public IEnumerable<ScoContent> GetSeminars(string seminarLicenseScoId, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var sessionResult = provider.GetContentsByScoId(seminarLicenseScoId);
            return sessionResult.Values.Where(x => x.Type == "meeting" && x.IsSeminar);
        }

        public IEnumerable<ScoContent> GetSeminarSessions(string seminarScoId, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var sessionResult = provider.GetContentsByScoId(seminarScoId);
            return sessionResult.Values.Where(x => x.Type == "seminarsession");
        }

        public ScoInfo CreateSeminar(SeminarUpdateItem seminar, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (seminar == null)
                throw new ArgumentNullException(nameof(seminar));
            if (!string.IsNullOrWhiteSpace(seminar.ScoId))
                throw new ArgumentException("sco-id should be empty.", nameof(seminar));

            ScoInfoResult result = provider.CreateSco(seminar);
            return ProcessResult(result);
        }

        public ScoInfo SaveSeminar(SeminarUpdateItem seminar, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (seminar == null)
                throw new ArgumentNullException(nameof(seminar));

            if (string.IsNullOrWhiteSpace(seminar.ScoId))
                throw new ArgumentException("sco-id can't be empty.", nameof(seminar));

            if (!string.IsNullOrWhiteSpace(seminar.UrlPath))
                throw new InvalidOperationException("UrlPath can't be updated.");
            else
                seminar.UrlPath = null;

            ScoInfoResult result = provider.UpdateSco(seminar);
            return ProcessResult(result);
        }

        public ScoInfo SaveSession(SeminarSessionDto sessionItem, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (sessionItem == null)
                throw new ArgumentNullException(nameof(sessionItem));
            if (sessionItem.ExpectedLoad <= 0)
                throw new ArgumentException("ExpectedLoad should have positive value", nameof(sessionItem));

            var isNewSession = string.IsNullOrWhiteSpace(sessionItem.SeminarSessionScoId);
            var session = new SeminarSessionUpdateItem
            {
                ScoId = sessionItem.SeminarSessionScoId,
                FolderId = sessionItem.SeminarScoId,
                Type = ScoType.seminarsession,
                Name = sessionItem.Name,
                Description = sessionItem.Summary
            };

            ScoInfoResult sessionScoResult = isNewSession ? provider.CreateSco(session) : provider.UpdateSco(session);
            ScoInfo sessionSco = ProcessResult(sessionScoResult);

            var sessionSettingsResult = provider.SeminarSessionScoUpdate(new SeminarSessionScoUpdateItem
            {
                ScoId = sessionSco.ScoId,
                Name = sessionSco.Name,
                DateBegin = sessionItem.DateBegin.ToString(AcDateFormat),
                DateEnd = sessionItem.DateEnd.ToString(AcDateFormat),
                ParentAclId = sessionItem.SeminarScoId,
                SourceScoId = sessionItem.SeminarScoId,
            });

            // if session was not updated correctly, it's sco would appear in the list anyway 
            // with wrong parameters(dates) => deleting just created session in case of unsuccessful update
            if (!sessionSettingsResult.Success && isNewSession)
            {
                provider.DeleteSco(sessionSco.ScoId);
            }

            var result = ProcessResult(sessionSettingsResult);

            StatusInfo loadResult = provider.UpdateAclField(result.ScoId, AclFieldId.seminar_expected_load, sessionItem.ExpectedLoad.ToString());
            if ((loadResult.Code != StatusCodes.ok) && isNewSession)
            {
                provider.DeleteSco(sessionSco.ScoId);
            }

            return result;
        }

        public StatusInfo DeleteSesson(string seminarSessionScoId, IAdobeConnectProxy provider)
        {
            if (string.IsNullOrWhiteSpace(seminarSessionScoId))
                throw new ArgumentException("Empty sco-id value", nameof(seminarSessionScoId));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            return provider.DeleteSco(seminarSessionScoId);
        }

        public StatusInfo DeleteSeminar(string seminarScoId, IAdobeConnectProxy provider)
        {
            if (string.IsNullOrWhiteSpace(seminarScoId))
                throw new ArgumentException("Empty sco-id value", nameof(seminarScoId));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            return provider.DeleteSco(seminarScoId);
        }

        private static ScoInfo ProcessResult(ScoInfoResult result)
        {
            if (!result.Success || result.ScoInfo == null)
            {
                if ((result.Status.SubCode == StatusSubCodes.duplicate) && (result.Status.InvalidField == "name"))
                    throw new WarningMessageException(Resources.Messages.MeetingNotUniqueName);

                if ((result.Status.SubCode == StatusSubCodes.duplicate) && (result.Status.InvalidField == "url-path"))
                    throw new WarningMessageException(Resources.Messages.MeetingNotUniqueUrlPath);
                if (result.Status.SubCode == StatusSubCodes.session_schedule_conflict)
                    throw new WarningMessageException(Resources.Messages.SessionScheduleConflict);

                throw new WarningMessageException(result.Status.GetErrorInfo());
            }

            return result.ScoInfo;
        }

    }

}
