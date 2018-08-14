using System;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto.Kaltura;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("kaltura")]
    public class KalturaController : BaseApiController
    {
        private readonly KalturaService _kalturaService;
        private readonly ILmsLicenseAccessor _licenseAccessor;

        public KalturaController(ApplicationSettingsProvider settings,
            ILogger logger, KalturaService kalturaService, ILmsLicenseAccessor licenseAccessor) : base(settings, logger)
        {
            _kalturaService = kalturaService;
            _licenseAccessor = licenseAccessor;
        }

        [Route("session")]
        [HttpGet]
        [LmsAuthorizeBase]
        public async Task<OperationResultWithData<KalturaSessionDto>> GetSession()
        {
            var licenseDto = await _licenseAccessor.GetLicense();
            var enableKaltura = licenseDto.GetSetting<bool>(LmsLicenseSettingNames.EnableKaltura);
            if (!enableKaltura)
                return OperationResultWithData<KalturaSessionDto>.Error("Kaltura is not enabled for this license.");

            try
            {
                var kalturaSession = await _kalturaService.GetKalturaSession();

                return kalturaSession.ToSuccessResult();
            }
            catch (Exception exception)
            {
                Logger.Error("GetKalturaSession", exception);

                return OperationResultWithData<KalturaSessionDto>.Error("Can't get Kaltura Session.");
            }
        }
    }
}