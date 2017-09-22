using System;
using System.Linq;
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

            // ACLTI-1132 Blackboard: IsTeacher Functionality
            if (lmsCompany.LmsProviderId == (int)LmsProviderEnum.Blackboard && !string.IsNullOrWhiteSpace(param.custom_role))
            {
                var isTeacher = lmsCompany.RoleMappings
                    .Where(x => x.IsTeacherRole)
                    .Select(x => x.LmsRoleName)
                    .Any(x => param.custom_role.Equals(x.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (isTeacher)
                {
                    return isTeacher;
                }
            }

            return
                //defaultTeacherRoles
                (!string.IsNullOrWhiteSpace((string)settings.TeacherRoles) && ((string)settings.TeacherRoles).Split(',')
                    .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0))
                //licenseSpecificTeacherRoles
                || lmsCompany.RoleMappings.Where(x => x.IsTeacherRole).Select(x => x.LmsRoleName)
                    .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

    }

}
