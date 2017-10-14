using System;
using System.Collections.Generic;
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


        public override IEnumerable<LmsCourseSectionDTO> GetCourseSections(ILmsLicense lmsLicense, string courseId)
        {
            return _canvasApi.GetCourseSections(lmsLicense.LmsDomain,
                lmsLicense.AdminUser.Token,
                int.Parse(courseId));
        }

    }

}