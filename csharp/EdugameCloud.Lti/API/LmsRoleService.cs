using System;
using System.Linq;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API
{
    public class LmsRoleService
    {
        private readonly string _teacherRoles;


        public LmsRoleService(ApplicationSettingsProvider settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _teacherRoles = (string)(settings as dynamic).TeacherRoles;
        }


        public bool IsTeacher(ILtiParam param, ILmsLicense lmsCompany)
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

            //LTI PARAMs from canvas do not contain custom roles.
            if (lmsCompany.LmsProviderId == (int)LmsProviderEnum.Canvas)
            {
                char[] delimiterChars = { '/', ',', '.', ':'};
                string roles = System.Web.HttpUtility.UrlDecode(param.roles);
                string[] lmsRolesFromParams = roles.Split(delimiterChars);

                return
                    //defaultTeacherRoles
                    (!string.IsNullOrWhiteSpace(_teacherRoles) && _teacherRoles.Split(',')
                        .Any(x => lmsRolesFromParams.Any(p => p.Equals(x.Trim(), StringComparison.InvariantCultureIgnoreCase))))
                    //licenseSpecificTeacherRoles
                    || lmsCompany.RoleMappings.Where(x => x.IsTeacherRole).Select(x => x.LmsRoleName)
                        .Any(x => lmsRolesFromParams.Any(p => p.Equals(x.Trim(), StringComparison.InvariantCultureIgnoreCase)));
            }

            return
                //defaultTeacherRoles
                (!string.IsNullOrWhiteSpace(_teacherRoles) && _teacherRoles.Split(',')
                    .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0))
                //licenseSpecificTeacherRoles
                || lmsCompany.RoleMappings.Where(x => x.IsTeacherRole).Select(x => x.LmsRoleName)
                    .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

    }

}
