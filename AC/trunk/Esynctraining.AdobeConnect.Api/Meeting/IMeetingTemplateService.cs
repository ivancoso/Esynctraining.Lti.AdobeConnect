using System;
using System.Collections.Generic;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;

namespace Esynctraining.AdobeConnect.Api.Meeting
{
    public interface IMeetingTemplateService
    {
        IEnumerable<TemplateDto> GetSharedMeetingTemplates(IAdobeConnectProxy provider, ICache cache, Func<string> cacheKeyFactory);

    }

}
