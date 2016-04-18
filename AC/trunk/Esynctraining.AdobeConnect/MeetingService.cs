using System.Collections.Generic;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect
{
    public class MeetingService : IMeetingService
    {
        private readonly ILogger _logger;
        private readonly IAdobeConnectAccountService _acAccountService;
        private readonly IAdobeConnectProxy _provider;


        public MeetingService(ILogger logger, IAdobeConnectAccountService acAccountService, IAdobeConnectAccess2 creds)
        {
            _logger = logger;
            _acAccountService = acAccountService;

            _provider = _acAccountService.GetProvider2(creds);
        }


        public IEnumerable<ScoContent> GetUserMeetings()
        {
            var shortcut = _provider.GetShortcutByType("my-meetings");

            if (shortcut == null)
                throw new WarningMessageException("User is not Meeting Host");

            ScoContentCollectionResult result = _provider.GetContentsByScoId(shortcut.ScoId);

            return result.Values;
        }
        
        public IEnumerable<ScoContent> GetSharedMeetings()
        {
            var shortcut = _provider.GetShortcutByType("meetings");

            ScoContentCollectionResult result = _provider.GetContentsByScoId(shortcut.ScoId);

            return result.Values;
        }

    }

}
