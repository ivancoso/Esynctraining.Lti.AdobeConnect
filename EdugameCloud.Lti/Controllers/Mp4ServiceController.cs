using System;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;

namespace EdugameCloud.Lti.Controllers
{
    public partial class Mp4ServiceController : BaseController
    {
        public Mp4ServiceController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
        }


        [HttpPost]
        public virtual JsonResult Convert(string lmsProviderName, string recordingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                string licenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                {
                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");
                }

                // TODO: check that licence is MP4 really???
                // WHERE: ????

                var mp4Client = new Mp4ServiceTaskClient();

                DataTask task = mp4Client.Convert(new Task
                {
                    LicenseId = licenseKey,
                    ScoId = long.Parse(recordingId),
                }).Result;

                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ConvertToMP4", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult ConvertWithSubtitles(string lmsProviderName, string recordingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                string licenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                {
                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");
                }

                // TODO: check that licence is MP4 really???
                // WHERE: ????

                var mp4Client = new Mp4ServiceTaskClient();

                DataTask task = mp4Client.Convert(new Task
                {
                    LicenseId = licenseKey,
                    ScoId = long.Parse(recordingId),
                }).Result;

                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ConvertToMP4", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}