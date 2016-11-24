using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect
{
    public class ContentService : IContentService
    {
        private readonly ILogger _logger;
        private readonly IAdobeConnectProxy _provider;


        public ContentService(ILogger logger, IAdobeConnectAccountService acAccountService, IAdobeConnectAccess2 creds)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (acAccountService == null)
                throw new ArgumentNullException(nameof(acAccountService));
            if (creds == null)
                throw new ArgumentNullException(nameof(creds));

            _logger = logger;
            _provider = acAccountService.GetProvider2(creds);
        }

        public ContentService(ILogger logger, IAdobeConnectProxy provider)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _logger = logger;
            _provider = provider;
        }


        public IEnumerable<ScoShortcut> GetShortcuts(IEnumerable<ScoShortcutType> typesToReturn)
        {
            if (typesToReturn == null)
                throw new ArgumentNullException(nameof(typesToReturn));

            // TRICK: to change 'my_meetings' -> 'my-meetings'
            IEnumerable<string> typeName = typesToReturn.Select(x => x.ToString().Replace("_", "-"));

            var shortcuts = _provider.GetShortcuts();
            return shortcuts.Where(s => typeName.Contains(s.Type));
        }

        [Obsolete] //??
        public IEnumerable<ScoContent> GetMyContent()
        {
            var shortcut = _provider.GetShortcutByType("my-content");

            if (shortcut == null)
                throw new WarningMessageException("User is not Meeting Host");

            ScoContentCollectionResult result = _provider.GetContentsByScoId(shortcut.ScoId);

            return result.Values;
        }

        [Obsolete] //??
        public ScoContent GetUserContentFolder(string userLogin)
        {
            if (string.IsNullOrWhiteSpace(userLogin))
                throw new ArgumentException("Non-empty value expected", nameof(userLogin));

            var shortcut = _provider.GetShortcutByType("user-content");

            ScoContentCollectionResult userFolders = _provider.GetContentsByScoId(shortcut.ScoId);
            var requestedUserFolder = userFolders.Values.FirstOrDefault(x => x.Name == userLogin);
            if (requestedUserFolder == null)
                throw new WarningMessageException("Requested user's folder not found. User is not Meeting Host.");

            return requestedUserFolder;
        }

        public IEnumerable<ScoContent> GetUserContent(string userLogin)
        {
            if (string.IsNullOrWhiteSpace(userLogin))
                throw new ArgumentException("Non-empty value expected", nameof(userLogin));

            var requestedUserFolder = GetUserContentFolder(userLogin);
            ScoContentCollectionResult result = _provider.GetContentsByScoId(requestedUserFolder.ScoId);
            return result.Values;
        }

        [Obsolete] //??
        public IEnumerable<ScoContent> GetSharedContent()
        {
            var shortcut = _provider.GetShortcutByType("content");

            ScoContentCollectionResult result = _provider.GetContentsByScoId(shortcut.ScoId);

            return result.Values;
        }

        public IEnumerable<ScoContent> GetFolderContent(string folderScoId)
        {
            if (string.IsNullOrWhiteSpace(folderScoId))
                throw new ArgumentException("Folder's sco-id should have value", nameof(folderScoId));

            ScoContentCollectionResult result = _provider.GetContentsByScoId(folderScoId);

            return result.Values;
        }

        public string GetDownloadAsZipLink(string scoId, string breezeToken)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            if (string.IsNullOrWhiteSpace(breezeToken))
                throw new ArgumentException("Non-empty value expected", nameof(breezeToken));

            ScoInfo scoInfo = DoGetSco(scoId);

            string acDomain = _provider.ApiUrl.Replace(@"api/xml", string.Empty).Trim('/');
            string cleanUrlPath = scoInfo.UrlPath.Trim('/');
            string fileExtention = "zip";// $".{scoInfo.Icon}"; ppt\pptx?
            
            return $"{acDomain}/{cleanUrlPath}/output/{cleanUrlPath}.{fileExtention}?download={fileExtention}&session={breezeToken}";
        }

        private ScoInfo DoGetSco(string scoId)
        {
            // check is user already has read permission!!!
            // TODO: setup only if source recording is accessible??
            //  ac.UpdateScoPermissionForPrincipal(scoId, principalId, MeetingPermissionId.view);
            var sco = _provider.GetScoInfo(scoId);
            if (sco.Status.Code == StatusCodes.no_access && sco.Status.SubCode == StatusSubCodes.denied)
            {
                _logger.ErrorFormat("DoGetSco: {0}. sco-id:{1}.", sco.Status.GetErrorInfo(), scoId);
                throw new WarningMessageException("Access denied.");
            }
            if (sco.Status.Code == StatusCodes.no_data && sco.Status.SubCode == StatusSubCodes.not_set)
            {
                _logger.ErrorFormat("DoGetSco: {0}. sco-id:{1}.", sco.Status.GetErrorInfo(), scoId);
                throw new WarningMessageException("File not found in Adobe Connect.");
            }
            else if (!sco.Success)
            {
                _logger.ErrorFormat("DoGetSco: {0}. sco-id:{1}.", sco.Status.GetErrorInfo(), scoId);
                string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.",
                 sco.Status.GetErrorInfo(),
                 scoId);
                throw new InvalidOperationException(msg);
            }

            return sco.ScoInfo;
        }

    }

}
