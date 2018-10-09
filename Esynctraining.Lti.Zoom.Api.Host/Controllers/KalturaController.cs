using System;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto.Kaltura;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("")]
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

        [Route("kaltura/session")]
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
                var kalturaSession = await _kalturaService.GetKalturaUserSession();

                return kalturaSession.ToSuccessResult();
            }
            catch (Exception exception)
            {
                Logger.Error("GetKalturaUserSession", exception);

                return OperationResultWithData<KalturaSessionDto>.Error("Can't get Kaltura Session.");
            }
        }

        //[HttpPost("meetings/{id}/kaltura-upload")]
        //public async Task<OperationResult> UploadVideoToPlayList(int id, [FromBody]VideoUploadDto request)
        //{
        //    try
        //    {
        //        var tags = new List<string>
        //        {
        //            id.ToString(),
        //            request.VideoTypeTag
        //        };

        //        var videoMediaEntry = await _kalturaService.ApproveMedia(request.Name,
        //            request.Description,
        //            tags,
        //            request.UploadedFileTokenId);

        //        Logger.Info($"Uploaded mediaId={videoMediaEntry?.Id}, meetingId={id}");
        //        return OperationResult.Success();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error($"UploadVideoToPlayList", ex);

        //        return OperationResult.Error("Server Exception during file upload.");
        //    }
        //}
    }
}