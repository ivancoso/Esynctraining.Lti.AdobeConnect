using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;

        public CanvasLmsCourseSectionsService(ILmsLicense license, LtiParamDTO param, IEGCEnabledCanvasAPI canvasApi) :
            base(license, param)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }

        public override async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()
        {
            IEnumerable<LmsCourseSectionDTO> result = await _canvasApi.GetCourseSections(License.LmsDomain,
                License.AdminUser.Token,
                Param.course_id);

            return result;
        }
    }
}