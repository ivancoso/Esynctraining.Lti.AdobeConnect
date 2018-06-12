using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API
{
    public class LmsCourseSectionsServiceBase
    {
        protected Dictionary<string, object> LicenseSettings { get; }
        protected LtiParamDTO Param { get; }

        public LmsCourseSectionsServiceBase(Dictionary<string, object> licenseSettings, LtiParamDTO param)
        {
            LicenseSettings = licenseSettings;
            Param = param;
        }

        public virtual Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()        {
            throw new NotImplementedException("LMS Sections are not supported for provider.");
        }
    }
}