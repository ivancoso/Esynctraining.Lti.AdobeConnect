using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Core.Utils
{
    public class CacheKeyUtil
    {
        public static string GetKey(LmsUserSession session)
        {
            return $"LMC_{session.LmsCompany.Id}_{session.LtiSession.LtiParam.lms_user_id}_AC";
        }
    }
}