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


        public CanvasLmsCourseSectionsService(IEGCEnabledCanvasAPI canvasApi)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }


        public override Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSectionsAsync(ILmsLicense lmsLicense, string courseId)
        {
            IEnumerable<LmsCourseSectionDTO> result = _canvasApi.GetCourseSections(lmsLicense.LmsDomain,
                lmsLicense.AdminUser.Token,
                int.Parse(courseId));

            return Task.FromResult(result);
        }

    }

}