using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Canvas
{
    public class CanvasLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;

        public CanvasLmsCourseSectionsService(Dictionary<string, object> licenseSettings, ILtiParam param, IEGCEnabledCanvasAPI canvasApi) :
            base(licenseSettings, param)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }

        public override async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()
        {
            IEnumerable<LmsCourseSectionDTO> result = await _canvasApi.GetCourseSections((string)LicenseSettings[LmsLicenseSettingNames.LmsDomain],
                (string)LicenseSettings[LmsUserSettingNames.Token],
                Param.course_id);

            return result;
        }
    }
}