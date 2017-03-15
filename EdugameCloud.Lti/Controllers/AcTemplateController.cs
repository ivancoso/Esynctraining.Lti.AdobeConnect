using System;
using System.Collections.Generic;
using System.Web.Http;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.AdobeConnect.Api.Meeting.Dto;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Controllers
{
    public class AcTemplateController : BaseApiController
    {
        public AcTemplateController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
        }


        // TODO: Add caching
        // TODO: copy DTO validation from SSO
        [HttpPost]
        [Route("templates")]
        public virtual OperationResultWithData<IEnumerable<TemplateDto>> GetTemplates([FromBody]TemplatesRequestDto request)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                credentials = session.LmsCompany;
                var scoShortcut =
                    MeetingTypeFactory.GetTemplatesShortcut(request.LmsMeetingType > 0
                        ? (LmsMeetingType) request.LmsMeetingType
                        : LmsMeetingType.Meeting);
                var api = this.GetAdobeConnectProvider(credentials);
                IEnumerable<TemplateDto> templates = new MeetingTemplateService(Logger).GetCachedTemplates(api, Cache,
                    () => CachePolicies.Keys.SharedMeetingTemplates(api.AdobeConnectRoot.ToString(), scoShortcut.ToString()), scoShortcut);

                return templates.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetTemplates", credentials, ex);
                return OperationResultWithData<IEnumerable<TemplateDto>>.Error(errorMessage);
            }
        }

    }

}