using System;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
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

        public KalturaController(ApplicationSettingsProvider settings,
            ILogger logger, KalturaService kalturaService) : base(settings, logger)
        {
            _kalturaService = kalturaService;
        }

        [Route("session")]
        [HttpGet]
        [LmsAuthorizeBase]
        public async Task<OperationResultWithData<KalturaSessionDto>> GetSession()
        {
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