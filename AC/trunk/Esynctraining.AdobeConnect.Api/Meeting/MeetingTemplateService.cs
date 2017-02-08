using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Utils;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Api.Meeting
{
    public class MeetingTemplateService : IMeetingTemplateService
    {
        private readonly ILogger _logger;


        public MeetingTemplateService(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }


        public IEnumerable<TemplateDto> GetSharedMeetingTemplates(IAdobeConnectProxy provider, ICache cache, Func<string> cacheKeyFactory)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            //CachePolicies.Keys.SharedMeetingTemplates(provider.ApiUrl)
            var item = CacheUtility.GetCachedItem<IEnumerable<TemplateDto>>(cache, cacheKeyFactory(),
                () => GetShortcutContentsFromApi(provider, ScoShortcutType.shared_meeting_templates.GetACEnum()));

            return item;
        }

        public IEnumerable<TemplateDto> GetSharedMeetingTemplates(IAdobeConnectProxy api)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));

            return GetShortcutContentsFromApi(api, ScoShortcutType.shared_meeting_templates.GetACEnum());
        }

        public IEnumerable<TemplateDto> GetMyMeetingTemplates(IAdobeConnectProxy api)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            return GetShortcutContentsFromApi(api, ScoShortcutType.my_meeting_templates.GetACEnum());
        }

        private IEnumerable<TemplateDto> GetShortcutContentsFromApi(IAdobeConnectProxy api, string shortcut)
        {
            ScoContentCollectionResult templates = api.GetContentsByType(shortcut);
            if (!templates.Success || templates.Values == null)
            {
                _logger.ErrorFormat("get {0}. AC error. {1}.", shortcut, templates.Status.GetErrorInfo());
                return Enumerable.Empty<TemplateDto>();
            }

            return templates.Values.Select(v => new TemplateDto { Id = v.ScoId, Name = v.Name }).ToList();
        }
    }
}
