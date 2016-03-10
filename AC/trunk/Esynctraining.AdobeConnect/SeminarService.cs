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

        
        public IEnumerable<ScoContent> GetSeminarLicenses(IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            ScoShortcut seminarShortcut = provider.GetShortcutByType("seminars");
            var result = provider.GetContentsByScoId(seminarShortcut.ScoId);
            return result.Values;
        }

        public IEnumerable<ScoContent> GetSeminars(string seminarLicenseScoId, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            var sessionResult = provider.GetContentsByScoId(seminarLicenseScoId);
            return sessionResult.Values.Where(x => x.Type == "meeting" && x.IsSeminar);
        }

        public IEnumerable<ScoContent> GetSeminarSessions(string seminarScoId, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            var sessionResult = provider.GetContentsByScoId(seminarScoId);
            return sessionResult.Values.Where(x => x.Type == "seminarsession");
        }

        public ScoInfo CreateSeminar(SeminarUpdateItem seminar, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (seminar == null)
                throw new ArgumentNullException("seminar");
            if (!string.IsNullOrWhiteSpace(seminar.ScoId))
                throw new ArgumentException("sco-id should be empty.", "seminar");

            ScoInfoResult result = provider.CreateSco(seminar);
            return ProcessResult(result);
        }

        public ScoInfo SaveSeminar(SeminarUpdateItem seminar, IAdobeConnectProxy provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (seminar == null)
                throw new ArgumentNullException("seminar");

            if (string.IsNullOrWhiteSpace(seminar.ScoId))
                throw new ArgumentException("sco-id can't be empty.", "seminar");

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
                throw new ArgumentNullException("provider");
            if (sessionItem == null)
                throw new ArgumentNullException("sessionItem");
            if (sessionItem.ExpectedLoad.HasValue && (sessionItem.ExpectedLoad.Value <= 0))
                throw new ArgumentException("ExpectedLoad should have positive value", "sessionItem");

            var session = new SeminarSessionUpdateItem
            {
                ScoId = sessionItem.SeminarSessionScoId,
                FolderId = sessionItem.SeminarScoId,
                Type = ScoType.seminarsession,
                Name = sessionItem.Name,
            };

            ScoInfoResult sessionScoResult = string.IsNullOrWhiteSpace(sessionItem.SeminarSessionScoId) ? provider.CreateSco(session) : provider.UpdateSco(session);
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

            var result = ProcessResult(sessionSettingsResult);

            if (sessionItem.ExpectedLoad.HasValue)
                provider.UpdateAclField(result.ScoId, AclFieldId.seminar_expected_load, sessionItem.ExpectedLoad.Value.ToString());

            return result;
        }

        public StatusInfo DeleteSesson(string seminarSessionScoId, IAdobeConnectProxy provider)
        {
            if (string.IsNullOrWhiteSpace(seminarSessionScoId))
                throw new ArgumentException("Empty sco-id value", "seminarSessionScoId");
            if (provider == null)
                throw new ArgumentNullException("provider");

            return provider.DeleteSco(seminarSessionScoId);
        }

        public StatusInfo DeleteSeminar(string seminarScoId, IAdobeConnectProxy provider)
        {
            if (string.IsNullOrWhiteSpace(seminarScoId))
                throw new ArgumentException("Empty sco-id value", "seminarScoId");
            if (provider == null)
                throw new ArgumentNullException("provider");

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

                throw new WarningMessageException(result.Status.GetErrorInfo());
            }

            return result.ScoInfo;
        }

    }

}
