using System;
using System.Collections.Generic;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        private readonly IEGCEnabledCanvasAPI canvasApi;
        private readonly ILogger logger;
        public CanvasLmsCourseSectionsService(IEGCEnabledCanvasAPI canvasApi, ILogger logger)
        {
            this.canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override IEnumerable<LmsCourseSectionDTO> GetCourseSections(ILmsLicense lmsLicense, string courseId)
        {
            return canvasApi.GetCourseSections(lmsLicense.LmsDomain,
                lmsLicense.AdminUser.Token,
                int.Parse(courseId));
        }
    }
}