using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.DataObjects.Results;
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
            var item = CacheUtility.GetCachedItem<IEnumerable<TemplateDto>>(cache, cacheKeyFactory(), () =>
            {
                ScoContentCollectionResult sharedTemplates = provider.GetContentsByType("shared-meeting-templates");
                if (!sharedTemplates.Success)
                {
                    _logger.ErrorFormat("get shared-meeting-templates. AC error. {0}.", sharedTemplates.Status.GetErrorInfo());
                    return Enumerable.Empty<TemplateDto>();
                }

                ScoContentCollectionResult result = provider.GetContentsByScoId(sharedTemplates.ScoId);
                if (result.Values == null)
                {
                    _logger.ErrorFormat("get shared-meeting-templates. AC error. {0}.", result.Status.GetErrorInfo());
                    return Enumerable.Empty<TemplateDto>();
                }

                return result.Values.Select(v => new TemplateDto { Id = v.ScoId, Name = v.Name }).ToList();
            });

            return item;
        }

    }

}
