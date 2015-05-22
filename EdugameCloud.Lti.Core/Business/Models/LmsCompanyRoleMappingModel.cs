namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EdugameCloud.Lti.DTO;
    using Newtonsoft.Json;

    public sealed class LmsCompanyRoleMappingModel  // : BaseModel<LmsCompanyRoleMapping, int>
    {
        private static readonly Dictionary<int, LmsCompanyRoleMappingDTO[]> _mappings;


        static LmsCompanyRoleMappingModel()
        {
            string config = "lms.companyRoleMapping.cfg.json";
            string mappingConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config);

            _mappings = JsonConvert.DeserializeObject<Dictionary<int, LmsCompanyRoleMappingDTO[]>>(File.ReadAllText(mappingConfig));
        }

        
        public IEnumerable<LmsCompanyRoleMappingDTO> GetDefaultMapping(int providerId)
        {
            LmsCompanyRoleMappingDTO[] result;
            if (_mappings.TryGetValue(providerId, out result))
                return result;

            return Enumerable.Empty<LmsCompanyRoleMappingDTO>();
        }

    }

}