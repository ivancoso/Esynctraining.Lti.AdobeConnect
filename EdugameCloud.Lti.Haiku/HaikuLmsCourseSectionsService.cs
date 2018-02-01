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


        public HaikuLmsCourseSectionsService(IHaikuRestApiClient apiClient)
        {
            _haikuApi = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }


        public override Task<IEnumerable<LmsCourseSectionDTO>> GetCourseSectionsAsync(ILmsLicense lmsLicense, string courseId)
        {
            return _haikuApi.GetCourseSectionsAsync(lmsLicense, int.Parse(courseId));
        }

    }

}
