using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Haiku;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuLmsCourseSectionsService : LmsCourseSectionsServiceBase
    {
        private readonly IHaikuRestApiClient _haikuApi;

        public HaikuLmsCourseSectionsService(ILmsLicense license, LtiParamDTO param, IHaikuRestApiClient apiClient) :
            base(license, param)
        {
            _haikuApi = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public override async Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSections()
        {
            return await _haikuApi.GetCourseSectionsAsync(License, Param.course_id);
        }
    }
}
