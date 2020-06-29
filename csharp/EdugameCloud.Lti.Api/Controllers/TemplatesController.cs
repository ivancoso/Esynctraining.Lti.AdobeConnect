using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class TemplatesController : BaseApiController
    {
        [DataContract]
        public class TemplatesRequestDto
        {
            [Required]
            [DataMember]
            public int LmsMeetingType { get; set; }
        }

        public TemplatesController(
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, 
            ICache cache
        )
            : base(acAccountService, settings, logger, cache)
        {
        }
        

        [HttpPost]
        [Route("templates")]
        [Filters.LmsAuthorizeBase]
        public virtual OperationResultWithData<IEnumerable<TemplateDto>> GetTemplates([FromBody]TemplatesRequestDto request)
        {
            try
            {
                var api = GetAdminProvider();
                var scoShortcut =
                    MeetingTypeFactory.GetTemplatesShortcut((LmsMeetingType)request.LmsMeetingType);

                IEnumerable<TemplateDto> templates = new MeetingTemplateService(Logger).GetCachedTemplates(api, Cache,
                    () => CachePolicies.Keys.SharedMeetingTemplates(api.AdobeConnectRoot.ToString(), scoShortcut.ToString()), scoShortcut);

                return templates.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetTemplates", ex);
                return OperationResultWithData<IEnumerable<TemplateDto>>.Error(errorMessage);
            }
        }

    }

}