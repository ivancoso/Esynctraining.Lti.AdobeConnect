using System;
using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;

namespace Esynctraining.AdobeConnect.Api.Meeting
{
    public interface IMeetingTemplateService
    {
        IEnumerable<TemplateDto> GetCachedTemplates(IAdobeConnectProxy api, ICache cache,
            Func<string> cacheKeyFactory, ScoShortcutType scoShortcut);

        IEnumerable<TemplateDto> GetSharedMeetingTemplates(IAdobeConnectProxy api);

        IEnumerable<TemplateDto> GetSharedMeetingTemplates(IAdobeConnectProxy api, ICache cache,
            Func<string> cacheKeyFactory);

        IEnumerable<TemplateDto> GetMyMeetingTemplates(IAdobeConnectProxy api);

    }

}
