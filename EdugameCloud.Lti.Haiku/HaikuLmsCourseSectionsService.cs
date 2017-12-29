﻿using System;
using System.Collections.Generic;
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


        public override IEnumerable<LmsCourseSectionDTO> GetCourseSections(ILmsLicense lmsLicense, string courseId)
        {
            return _haikuApi.GetCourseSections(lmsLicense, int.Parse(courseId), out string error);
        }

    }

}
