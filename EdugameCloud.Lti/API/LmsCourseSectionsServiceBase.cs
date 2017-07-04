using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API
{
    public class LmsCourseSectionsServiceBase
    {
        public virtual IEnumerable<LmsCourseSectionDTO> GetCourseSections(ILmsLicense lmsLicense, string courseId)
        {
            throw new NotImplementedException("LMS Sections are not supported for provider.");
        }
    }
}