namespace Esynctraining.Lti.Lms.Common.API
{
    //public class LmsRoleService
    //{
    //    private readonly string _teacherRoles;


    //    public LmsRoleService(ApplicationSettingsProvider settings)
    //    {
    //        if (settings == null)
    //            throw new ArgumentNullException(nameof(settings));

    //        _teacherRoles = (string)(settings as dynamic).TeacherRoles;
    //    }


    //    public bool IsTeacher(LtiParamDTO param, Dictionary<string, object> licenseSettings)
    //    {
    //        if (param == null)
    //            throw new ArgumentNullException(nameof(param));
    //        if (licenseSettings == null)
    //            throw new ArgumentNullException(nameof(licenseSettings));

    //        if (param.roles == null)
    //            return false;

    //        // ACLTI-1132 Blackboard: IsTeacher Functionality
    //        if ((int)licenseSettings["LmsProviderId"] == (int)LmsProviderEnum.Blackboard && !string.IsNullOrWhiteSpace(param.custom_role))
    //        {
    //            var isTeacher = lmsCompany.RoleMappings
    //                .Where(x => x.IsTeacherRole)
    //                .Select(x => x.LmsRoleName)
    //                .Any(x => param.custom_role.Equals(x.Trim(), StringComparison.InvariantCultureIgnoreCase));

    //            if (isTeacher)
    //            {
    //                return isTeacher;
    //            }
    //        }

    //        return
    //            //defaultTeacherRoles
    //            (!string.IsNullOrWhiteSpace(_teacherRoles) && _teacherRoles.Split(',')
    //                .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0))
    //            //licenseSpecificTeacherRoles
    //            || lmsCompany.RoleMappings.Where(x => x.IsTeacherRole).Select(x => x.LmsRoleName)
    //                .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
    //    }

    //}

}
