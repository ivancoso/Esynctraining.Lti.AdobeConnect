using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.API
{
    public class LmsRoleService
    {
        private readonly dynamic settings;


        public LmsRoleService(ApplicationSettingsProvider settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }


        public bool IsTeacher(LtiParamDTO param, ILmsLicense lmsCompany)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            if (param.roles == null)
                return false;

            IEnumerable<string> defaultTeacherRoles = ((string)settings.TeacherRoles).Split(',');
            IEnumerable<string> licenseSpecificTeacherRoles = lmsCompany.RoleMappings.Where(x => x.IsTeacherRole).Select(x => x.LmsRoleName);

            return
                //defaultTeacherRoles
                ((string)settings.TeacherRoles).Split(',')
                    .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                //licenseSpecificTeacherRoles
                || lmsCompany.RoleMappings.Where(x => x.IsTeacherRole).Select(x => x.LmsRoleName)
                    .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

    }

}
