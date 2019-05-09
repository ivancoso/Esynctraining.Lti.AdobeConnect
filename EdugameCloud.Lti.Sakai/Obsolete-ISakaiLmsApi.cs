//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Esynctraining.Lti.Lms.Common.Dto;

//namespace EdugameCloud.Lti.Sakai
//{
//    public interface ISakaiLmsApi
//    {
//        Task<(List<LmsUserDTO> Data, string Error)> GetUsersForCourseAsync(
//            Dictionary<string, object> licenseSettings,
//            string courseid);

//        Task<(bool Data, string Error)> LoginAndCheckSessionAsync(
//            bool useSsl,
//            string lmsDomain,
//            string userName,
//            string password,
//            bool recursive = false);
//    }
//}