using System;
using System.Linq;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Core.Business
{
    public sealed class RoleMappingService
    {
        public MeetingPermissionId SetAcRole(ILmsLicense lmsCompany, LmsUserDTO u, bool ignoreEmptyACRole = false)
        {
            string role = u.LmsRole != null ? u.LmsRole.ToLower() : string.Empty;

            var permission = MeetingPermissionId.view;
            u.AcRole = AcRole.Participant.Id;
            if (string.IsNullOrWhiteSpace(u.Id) || u.Id.Equals("0"))
            {
                permission = MeetingPermissionId.remove;
                u.AcRole = null; // "Remove"; // probably doesn't make sence, need to review and remove
            }

            LmsCompanyRoleMapping mapping = lmsCompany.RoleMappings.FirstOrDefault(x => x.LmsRoleName.Equals(role, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                if (mapping.AcRole == AcRole.None.Id)
                {
                    if (!ignoreEmptyACRole)
                    {
                        permission = AcRole.None.MeetingPermissionId;
                        u.AcRole = AcRole.None.Id;
                    }

                    return permission;
                }
                else
                {
                    AcRole acRole = AcRole.GetById(mapping.AcRole);
                    u.AcRole = acRole.Id;
                    return acRole.MeetingPermissionId;
                }
            }

            if (role.Contains("teacher") || role.Contains("instructor") || role.Contains("owner")
                || role.Contains("admin") || role.Contains("lecture"))
            {
                permission = MeetingPermissionId.host;
                u.AcRole = AcRole.Host.Id;
            }
            else if (role.Contains("ta") || role.Contains("designer") || role.Contains("author")
                     || role.Contains("teaching assistant") || role.Contains("course builder")
                     || role.Contains("grader") || role == "advisor")
            {
                u.AcRole = AcRole.Presenter.Id;
                permission = MeetingPermissionId.mini_host;
            }

            return permission;
        }

        public void CheckAndSetNoneACMapping(LmsUserDTO user, ILmsLicense lmsCompany)
        {
            LmsCompanyRoleMapping mapping = lmsCompany.RoleMappings
                        .FirstOrDefault(x => x.LmsRoleName.Equals(user.LmsRole, StringComparison.OrdinalIgnoreCase));
            if (mapping != null && mapping.AcRole == AcRole.None.Id) // LMS role is set to be not mapped to any AC role
            {
                user.AcRole = AcRole.None.Id;
            }
        }
    }
}
