namespace EdugameCloud.WCFService
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Utils;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReportingService : BaseService, IReportingService
    {
        #region Properties
        
        private AppletItemModel AppletItemModel => IoC.Resolve<AppletItemModel>();
        
        private QuizResultModel QuizResultModel => IoC.Resolve<QuizResultModel>();
        
        private TestResultModel TestResultModel => IoC.Resolve<TestResultModel>();
        
        private SurveyResultModel SurveyResultModel => IoC.Resolve<SurveyResultModel>();
        
        private SNGroupDiscussionModel SNGroupDiscussionModel => IoC.Resolve<SNGroupDiscussionModel>();
        
        private SNMemberModel SNMemberModel => IoC.Resolve<SNMemberModel>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get crossword result by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordResultByAcSessionFromStoredProcedureDTO"/>.
        /// </returns>
        public CrosswordResultByAcSessionDTO[] GetCrosswordResultByACSessionId(int sessionId)
        {
            var result = this.AppletItemModel.GetCrosswordResultByACSessionId(sessionId).ToList();
            return result.Select(x => new CrosswordResultByAcSessionDTO(x)).ToArray();
        }

        /// <summary>
        /// The get crossword sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordSessionFromStoredProcedureDTO"/>.
        /// </returns>
        public CrosswordSessionDTO[] GetCrosswordSessionsByUserId(int userId)
        {
            var result = this.AppletItemModel.GetCrosswordSessionsByUserId(userId).ToList();
            return result.Select(x => new CrosswordSessionDTO(x)).ToArray();
        }

        /// <summary>
        /// The get quiz result by ac session id.
        /// </summary>
        /// <param name="sessionId">
        /// The ac session id.
        /// </param>
        /// <param name="subModuleItemId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDataDTO"/>.
        /// </returns>
        public QuizResultDataDTO GetQuizResultByACSessionId(int sessionId, int subModuleItemId)
        {
            return this.QuizResultModel.GetQuizResultByACSessionId(sessionId, subModuleItemId);
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizSessionFromStoredProcedureDTO"/>.
        /// </returns>
        public QuizSessionDTO[] GetQuizSessionsByUserId(int userId)
        {
            return this.ACSessionModel.GetQuizSessionsByUserId(userId).Select(x => new QuizSessionDTO(x)).ToArray();
        }

        /// <summary>
        /// The get quiz result by ac session id.
        /// </summary>
        /// <param name="sessionId">
        /// The ac session id.
        /// </param>
        /// <param name="subModuleItemId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="TestResultDataDTO"/>.
        /// </returns>
        public TestResultDataDTO GetTestResultByACSessionId(int sessionId, int subModuleItemId)
        {
            return this.TestResultModel.GetTestResultByACSessionId(sessionId, subModuleItemId);
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="TestSessionDTO"/>.
        /// </returns>
        public TestSessionDTO[] GetTestSessionsByUserId(int userId)
        {
            return this.ACSessionModel.GetTestSessionsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz result by ac session id.
        /// </summary>
        /// <param name="sessionId">
        /// The ac session id.
        /// </param>
        /// <param name="subModuleItemId">
        /// The sub module item id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyResultDataDTO"/>.
        /// </returns>
        public SurveyResultDataDTO GetSurveyResultByACSessionId(int sessionId, int subModuleItemId)
        {
            return this.SurveyResultModel.GetSurveyResultByACSessionId(sessionId, subModuleItemId);
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <returns>
        /// The <see cref="SurveySessionFromStoredProcedureDTO"/>.
        /// </returns>
        public SurveySessionDTO[] TestDotAMF()
        {
            var result = this.ACSessionModel.GetSurveySessionsByUserId(32).ToList();
            var resultEx = result.Select(x => new SurveySessionDTO(x));
            return resultEx.ToArray();
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveySessionFromStoredProcedureDTO"/>.
        /// </returns>
        public SurveySessionDTO[] GetSurveySessionsByUserId(int userId)
        {
            var result = this.ACSessionModel.GetSurveySessionsByUserId(userId).ToList();
            var resultEx = result.Select(x => new SurveySessionDTO(x));
            return resultEx.ToArray();
        }

        /// <summary>
        /// The get SN sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SNSessionDTO"/>.
        /// </returns>
        public SNSessionDTO[] GetSNSessionsByUserId(int userId)
        {
            return this.ACSessionModel.GetSNSessionsByUserId(userId).ToList().Select(x => new SNSessionDTO(x)).ToArray();
        }

        /// <summary>
        /// The get recent splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="RecentReportDTO"/>.
        /// </returns>
        public PagedRecentReportsDTO GetRecentSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            return this.GetBaseRecentSplashScreenReports(userId, pageIndex, pageSize);
        }

        /// <summary>
        /// The get SN data by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="SNReportingDataDTO"/>.
        /// </returns>
        public SNReportingDataDTO GetSNDataByACSessionId(int sessionId)
        {
            var data = SNGroupDiscussionModel.GetOneByACSessionId(sessionId).Value;
            return new SNReportingDataDTO
                       {
                           discussion = data == null ? null : new SNGroupDiscussionDTO(data),
                           members =
                               SNMemberModel.GetAllByACSessionId(sessionId)
                               .Select(x => new SNMemberDTO(x))
                               .ToArray()
                       };
        }

        /// <summary>
        /// The get splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ReportDTO"/>.
        /// </returns>
        public PagedReportsDTO GetSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            return this.GetBaseSplashScreenReports(userId, pageIndex, pageSize);
        }

        /// <summary>
        /// The get splash screen.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SplashScreenDTO"/>.
        /// </returns>
        public SplashScreenDTO GetSplashScreen(int userId)
        {
            return new SplashScreenDTO
                       {
                           recentReports = this.GetRecentSplashScreenReports(userId, 1, 10).objects.ToArray(),
                           reports = this.GetSplashScreenReports(userId, 1, 5).objects.ToArray()
                       };
        }

        #endregion
    }
}