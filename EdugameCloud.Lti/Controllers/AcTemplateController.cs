﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Controllers
{
    public class AcTemplateController : BaseController
    {
        public AcTemplateController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
        }


        [HttpPost]
        public virtual JsonResult GetTemplates(string lmsProviderName)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                IEnumerable<TemplateDTO> templates = acAccountService.GetTemplates(
                    this.GetAdobeConnectProvider(session.LmsCompany),
                    session.LmsCompany.ACTemplateScoId);

                return Json(OperationResultWithData<IEnumerable<TemplateDTO>>.Success(templates));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetTemplates", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}