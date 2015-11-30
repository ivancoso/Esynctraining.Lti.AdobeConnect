namespace EdugameCloud.Lti.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.NHibernate;
    using Newtonsoft.Json;

    public sealed class LmsCompanyRoleMappingModel : BaseModel<LmsCompanyRoleMapping, int>
    {
        private static readonly Dictionary<int, LmsCompanyRoleMappingDTO[]> _mappings;


        static LmsCompanyRoleMappingModel()
        {
            string config = "lms.companyRoleMapping.cfg.json";
            string mappingConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config);

            _mappings = JsonConvert.DeserializeObject<Dictionary<int, LmsCompanyRoleMappingDTO[]>>(File.ReadAllText(mappingConfig));
            foreach (LmsCompanyRoleMappingDTO[] roleMapSet in _mappings.Values)
                foreach (LmsCompanyRoleMappingDTO roleMap in roleMapSet)
                    roleMap.isDefaultLmsRole = true;
        }


        public LmsCompanyRoleMappingModel() : base(null)
        { 
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