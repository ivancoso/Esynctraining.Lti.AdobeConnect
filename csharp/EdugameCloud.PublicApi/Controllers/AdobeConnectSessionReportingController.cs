using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using Esynctraining.Core.Utils;
using Microsoft.AspNet.Identity;

namespace EdugameCloud.PublicApi.Controllers
{
    /// <summary>
    /// TODO DESCRIPTION.
    /// </summary>
    [Authorize]
    [RoutePrefix("reporting")]
    [EnableCors(origins: "*", headers: "*", methods: "get,option")]
    public sealed class AdobeConnectSessionReportingController : ApiController
    {
        #region Properties

        private ACSessionModel ACSessionModel
        {
            get { return IoC.Resolve<ACSessionModel>(); }
        }

        private AppletItemModel AppletItemModel
        {
            get { return IoC.Resolve<AppletItemModel>(); }
        }

        private QuizResultModel QuizResultModel
        {
            get { return IoC.Resolve<QuizResultModel>(); }
        }

        private TestResultModel TestResultModel
        {
            get { return IoC.Resolve<TestResultModel>(); }
        }

        private SurveyResultModel SurveyResultModel
        {
            get { return IoC.Resolve<SurveyResultModel>(); }
        }

        private SNGroupDiscussionModel SNGroupDiscussionModel
        {
            get { return IoC.Resolve<SNGroupDiscussionModel>(); }
        }

        private SNMemberModel SNMemberModel
        {
            get { return IoC.Resolve<SNMemberModel>(); }
        }

        #endregion

        /// <summary>
        /// Returns crossword sessions for the calling user.
        /// </summary>
        /// <returns>Array of session entries.</returns>
        [HttpGet]
        [Route("crosswordsessions")]
        public IEnumerable<CrosswordSessionDTO> GetCrosswordSessionsByUserId()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return AppletItemModel.GetCrosswordSessionsByUserId(userId).Select(x => new CrosswordSessionDTO(x));
        }

        //[HttpGet]
        //[Route("crosswordsessions/by-meeting")]
        //public IEnumerable<CrosswordSessionDTO> GetCrosswordSessionsByUserId(string meetingUrl)
        //{
        //    var userId = int.Parse(User.Identity.GetUserId());
        //    return AppletItemModel.GetCrosswordSessionsByUserIdMeetingUrl(userId, meetingUrl).Select(x => new CrosswordSessionDTO(x));
        //}

        [HttpGet]
        [Route("crosswordsessions/{acSessionId:int:min(1)}")]
        public IEnumerable<CrosswordResultByAcSessionDTO> GetCrosswordResultByACSessionId(int acSessionId)
        {
            return AppletItemModel.GetCrosswordResultByACSessionId(acSessionId).Select(x => new CrosswordResultByAcSessionDTO(x));
        }

        /// <summary>
        /// Returns quiz sessions for the user.
        /// </summary>
        /// <returns>Array of session entries.</returns>
        [HttpGet]
        [Route("quizsessions")]
        public IEnumerable<QuizSessionDTO> GetQuizSessionsByUserId()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetQuizSessionsByUserId(userId).Select(x => new QuizSessionDTO(x));
        }

        [HttpGet]
        [Route("quizsessions/by-meeting")]
        public IEnumerable<QuizSessionDTO> GetQuizSessionsByMeetingUrl(string meetingUrl)
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetQuizSessionsByUserIdMeetingUrl(userId, meetingUrl).Select(x => new QuizSessionDTO(x));
        }

        [HttpGet]
        [Route("quizsessions/{acSessionId:int:min(1)}/{subModuleItemId:int:min(1)}")]
        public QuizResultDataDTO GetQuizResultByACSessionId(int acSessionId, int subModuleItemId)
        {
            return QuizResultModel.GetQuizResultByACSessionId(acSessionId, subModuleItemId);
        }

        [HttpGet]
        [Route("quizsessions/{acSessionId:int:min(1)}/{subModuleItemId:int:min(1)}/{adobeConnectEmail}")]
        public QuizResultDataDTO GetQuizResultByACSessionIdAcEmail(int acSessionId, int subModuleItemId, string adobeConnectEmail)
        {
            return QuizResultModel.GetQuizResultByACSessionIdAcEmail(acSessionId, subModuleItemId, adobeConnectEmail);
        }
        
        [HttpGet]
        [Route("snsessions")]
        public IEnumerable<SNSessionDTO> GetSNSessionsByUserId()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetSNSessionsByUserId(userId).Select(x => new SNSessionDTO(x));
        }

        [HttpGet]
        [Route("snsessions/{acSessionId:int:min(1)}")]
        public SNReportingDataDTO GetSNDataByACSessionId(int acSessionId)
        {
            var data = SNGroupDiscussionModel.GetOneByACSessionId(acSessionId).Value;
            return new SNReportingDataDTO
            {
                discussion = data == null ? null : new SNGroupDiscussionDTO(data),
                members =
                    SNMemberModel.GetAllByACSessionId(acSessionId)
                    .Select(x => new SNMemberDTO(x))
                    .ToArray()
            };
        }

        /// <summary>
        /// Returns survey sessions for the user.
        /// </summary>
        /// <returns>Array of session entries.</returns>
        [HttpGet]
        [Route("surveysessions")]
        public IEnumerable<SurveySessionDTO> GetSurveySessionsByUserId()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetSurveySessionsByUserId(userId).Select(x => new SurveySessionDTO(x));
        }

        [HttpGet]
        [Route("surveysessions/by-meeting")]
        public IEnumerable<SurveySessionDTO> GetSurveySessionsByMeetingUrl(string meetingUrl)
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetSurveySessionsByUserIdMeetingUrl(userId, meetingUrl).Select(x => new SurveySessionDTO(x));
        }

        [HttpGet]
        [Route("surveysessions/{acSessionId:int:min(1)}/{subModuleItemId:int:min(1)}")]
        public SurveyResultDataDTO GetSurveyResultByACSessionId(int acSessionId, int subModuleItemId)
        {
            return SurveyResultModel.GetSurveyResultByACSessionId(acSessionId, subModuleItemId);
        }

        [HttpGet]
        [Route("surveysessions/{acSessionId:int:min(1)}/{subModuleItemId:int:min(1)}/{adobeConnectEmail}")]
        public SurveyResultDataDTO GetSurveyResultByACSessionIdAcEmail(int acSessionId, int subModuleItemId, string adobeConnectEmail)
        {
            return SurveyResultModel.GetSurveyResultByACSessionIdAcEmail(acSessionId, subModuleItemId, adobeConnectEmail);
        }


        /// <summary>
        /// Returns test sessions for the user.
        /// </summary>
        /// <returns>Array of session entries.</returns>
        [HttpGet]
        [Route("testsessions")]
        public IEnumerable<TestSessionDTO> GetTestSessionsByUserId()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetTestSessionsByUserId(userId);
        }

        [HttpGet]
        [Route("testsessions/by-meeting")]
        public IEnumerable<TestSessionDTO> GetTestSessionsByMeetingUrl(string meetingUrl)
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return ACSessionModel.GetTestSessionsByUserIdMeetingUrl(userId, meetingUrl);
        }

        [HttpGet]
        [Route("testsessions/{acSessionId:int:min(1)}/{subModuleItemId:int:min(1)}")]
        public TestResultDataDTO GetTestResultByACSessionId(int acSessionId, int subModuleItemId)
        {
            return TestResultModel.GetTestResultByACSessionId(acSessionId, subModuleItemId);
        }

        [HttpGet]
        [Route("testsessions/{acSessionId:int:min(1)}/{subModuleItemId:int:min(1)}/{adobeConnectEmail}")]
        public TestResultDataDTO GetTestResultByACSessionIdAcEmail(int acSessionId, int subModuleItemId, string adobeConnectEmail)
        {
            return TestResultModel.GetTestResultByACSessionIdAcEmail(acSessionId, subModuleItemId, adobeConnectEmail);
        }

    }

}
