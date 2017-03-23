using System;
using System.Collections.Generic;
using EdugameCloud.Lti.API.AdobeConnect;
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
        public virtual OperationResultWithData<IEnumerable<TemplateDto>> GetTemplates()
        {
            try
            {
                IEnumerable<TemplateDto> templates = acAccountService.GetSharedMeetingTemplates(GetAdminProvider(), Cache);
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