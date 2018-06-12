using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;

        public CanvasLmsCourseSectionsService(Dictionary<string, object> licenseSettings, LtiParamDTO param, IEGCEnabledCanvasAPI canvasApi) :
            base(licenseSettings, param)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }

        public override async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()
        {
            IEnumerable<LmsCourseSectionDTO> result = await _canvasApi.GetCourseSections((string)LicenseSettings["LmsDomain"],
                (string)LicenseSettings["AdminUser.Token"],
                Param.course_id.ToString());

            return result;
        }
    }
}