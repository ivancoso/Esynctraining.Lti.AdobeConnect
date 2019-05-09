using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Dto;
//using Esynctraining.Lti.Lms.Sakai;

namespace EdugameCloud.Lti.Sakai
{
    public class SakaiLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        //private readonly LTI2Api _lti2Api;

        public SakaiLmsCourseSectionsService(Dictionary<string, object> licenseSettings, ILtiParam param) : //, LTI2Api lti2Api
            base(licenseSettings, param)
        {
            //_lti2Api = lti2Api ?? throw new ArgumentNullException(nameof(lti2Api));
        }

        public override async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()
        {
            //var result = await _lti2Api.GetUsersForCourse(
            //    License,
            //    Param.ext_ims_lis_memberships_url ?? Param.ext_ims_lti_tool_setting_url,
            //    Param.ext_ims_lis_memberships_id);

            return new List<LmsCourseSectionDTO>();
        }
    }
}