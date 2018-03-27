using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API
{
    public class LmsCourseSectionsServiceBase
    {
        protected ILmsLicense License { get; }
        protected LtiParamDTO Param { get; }

        public LmsCourseSectionsServiceBase(ILmsLicense license, LtiParamDTO param)
        {
            License = license;
            Param = param;
        }

        public virtual Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()        {
            throw new NotImplementedException("LMS Sections are not supported for provider.");
        }
    }
}