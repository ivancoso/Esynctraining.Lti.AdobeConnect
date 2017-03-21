using System;
using System.Collections.Generic;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
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
        //public AcTemplateController(
        //    LmsUserSessionModel userSessionModel,
        //    IAdobeConnectAccountService acAccountService,
        //    ApplicationSettingsProvider settings,
        //    ILogger logger, ICache cache)
        //    : base(userSessionModel, acAccountService, settings, logger, cache)
        //{
        //}


        // TODO: Add caching
        // TODO: copy DTO validation from SSO
        [HttpPost]
        [Route("templates")]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public virtual OperationResultWithData<IEnumerable<TemplateDto>> GetTemplates()
        {
            try
            {
                IEnumerable<TemplateDto> templates = acAccountService.GetSharedMeetingTemplates(GetAdobeConnectProvider(), Cache);
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