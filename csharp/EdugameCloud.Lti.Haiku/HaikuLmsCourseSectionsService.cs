using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Haiku;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        private readonly IHaikuRestApiClient _haikuApi;

        public HaikuLmsCourseSectionsService(Dictionary<string, object> licenseSettings, ILtiParam param, IHaikuRestApiClient apiClient) :
            base(licenseSettings, param)
        {
            _haikuApi = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public override async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()
        {
            return await _haikuApi.GetCourseSectionsAsync(LicenseSettings, Param.course_id.ToString());
        }
    }
}
