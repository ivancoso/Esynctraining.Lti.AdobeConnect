using System;
using System.Linq;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.Business
{
    public sealed class RoleMappingService
    {
        public MeetingPermissionId SetAcRole(LmsCompany lmsCompany, LmsUserDTO u)
        {
            string role = u.lms_role != null ? u.lms_role.ToLower() : string.Empty;

            var permission = MeetingPermissionId.view;
            u.ac_role = AcRole.Participant.Name;
            if (string.IsNullOrWhiteSpace(u.id) || u.id.Equals("0"))
            {
                permission = MeetingPermissionId.remove;
                u.ac_role = "Remove";
            }

            // NOTE: temporary disabled!!
            //LmsCompanyRoleMapping mapping = lmsCompany.RoleMappings.FirstOrDefault(x => x.LmsRoleName.Equals(role, StringComparison.OrdinalIgnoreCase));
            //if (mapping != null)
            //{
            //    AcRole acRole = AcRole.GetById(mapping.AcRole);
            //    u.ac_role = acRole.Name;
            //    return acRole.MeetingPermissionId;
            //}

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner")
                || role.Contains("admin") || role.Contains("lecture"))
            {
                permission = MeetingPermissionId.host;
                u.ac_role = AcRole.Host.Name;
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author")
                     || role.Contains("teaching assistant") || role.Contains("course builder")
                     || role.Contains("evaluator") || role == "advisor")
            {
                u.ac_role = AcRole.Presenter.Name;
                permission = MeetingPermissionId.mini_host;
            }

            return permission;
        }

    }

}
