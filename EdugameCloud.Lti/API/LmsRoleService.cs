using System;
using System.Linq;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.API
{
    public class LmsRoleService
    {
        private readonly dynamic settings;


        public LmsRoleService(ApplicationSettingsProvider settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            this.settings = settings;
        }


        public bool IsTeacher(LtiParamDTO param)
        {
            return param.roles != null
                && settings.TeacherRoles != null
                && ((string)settings.TeacherRoles).Split(',').Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

    }

}
