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


        public IEnumerable<ScoContent> GetUserContent()
        {
            var shortcut = _provider.GetShortcutByType("my-content");

            if (shortcut == null)
                throw new WarningMessageException("User is not Meeting Host");

            ScoContentCollectionResult result = _provider.GetContentsByScoId(shortcut.ScoId);

            return result.Values;
        }
        
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

    }

}
