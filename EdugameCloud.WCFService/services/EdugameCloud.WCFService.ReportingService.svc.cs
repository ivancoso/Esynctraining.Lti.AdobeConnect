// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Utils;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReportingService : BaseService, IReportingService
    {
        #region Properties

        /// <summary>
        /// Gets the applet item model.
        /// </summary>
        private AppletItemModel AppletItemModel
        {
            get
            {
                return IoC.Resolve<AppletItemModel>();
            }
        }

        /// <summary>
        /// Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        /// <summary>
        /// Gets the test result model.
        /// </summary>
        private TestResultModel TestResultModel
        {
            get
            {
                return IoC.Resolve<TestResultModel>();
            }
        }

        /// <summary>
        /// Gets the survey result model.
        /// </summary>
        private SurveyResultModel SurveyResultModel
        {
            get
            {
                return IoC.Resolve<SurveyResultModel>();
            }
        }

        /// <summary>
        /// Gets the SN group discussion model.
        /// </summary>
        private SNGroupDiscussionModel SNGroupDiscussionModel
        {
            get
            {
                return IoC.Resolve<SNGroupDiscussionModel>();
            }
        }

        /// <summary>
        /// Gets the SN member model.
        /// </summary>
        private SNMemberModel SNMemberModel
        {
            get
            {
                return IoC.Resolve<SNMemberModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get crossword result by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CrosswordResultByAcSessionDTO> GetCrosswordResultByACSessionId(int sessionId)
        {
            return new ServiceResponse<CrosswordResultByAcSessionDTO>
                       {
                           objects = this.AppletItemModel.GetCrosswordResultByACSessionId(sessionId).ToList()
                       };
        }

        /// <summary>
        /// The get crossword sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<CrosswordSessionDTO> GetCrosswordSessionsByUserId(int userId)
        {
            return new ServiceResponse<CrosswordSessionDTO>
                       {
                           objects = this.AppletItemModel.GetCrosswordSessionsByUserId(userId).ToList()
                       };
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizResultDataDTO> GetQuizResultByACSessionId(int sessionId, int subModuleItemId)
        {
            return new ServiceResponse<QuizResultDataDTO>
                       {
                           @object = this.QuizResultModel.GetQuizResultByACSessionId(sessionId, subModuleItemId)
                       };
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizSessionDTO> GetQuizSessionsByUserId(int userId)
        {
            return new ServiceResponse<QuizSessionDTO>
                       {
                           objects = this.ACSessionModel.GetQuizSessionsByUserId(userId).ToList()
                       };
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestResultDataDTO> GetTestResultByACSessionId(int sessionId, int subModuleItemId)
        {
            return new ServiceResponse<TestResultDataDTO>
            {
                @object = this.TestResultModel.GetTestResultByACSessionId(sessionId, subModuleItemId)
            };
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestSessionDTO> GetTestSessionsByUserId(int userId)
        {
            return new ServiceResponse<TestSessionDTO>
            {
                objects = this.ACSessionModel.GetTestSessionsByUserId(userId).ToList()
            };
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyResultDataDTO> GetSurveyResultByACSessionId(int sessionId, int subModuleItemId)
        {
            return new ServiceResponse<SurveyResultDataDTO>
            {
                @object = this.SurveyResultModel.GetSurveyResultByACSessionId(sessionId, subModuleItemId)
            };
        }

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveySessionDTO> GetSurveySessionsByUserId(int userId)
        {
            return new ServiceResponse<SurveySessionDTO>
            {
                objects = this.ACSessionModel.GetSurveySessionsByUserId(userId).ToList()
            };
        }

        /// <summary>
        /// The get SN sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNSessionDTO> GetSNSessionsByUserId(int userId)
        {
            return new ServiceResponse<SNSessionDTO>
            {
                objects = this.ACSessionModel.GetSNSessionsByUserId(userId).ToList()
            };
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<RecentReportDTO> GetRecentSplashScreenReports(int userId, int pageIndex, int pageSize)
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SNReportingDataDTO> GetSNDataByACSessionId(int sessionId)
        {
            var data = SNGroupDiscussionModel.GetOneByACSessionId(sessionId).Value;
            return new ServiceResponse<SNReportingDataDTO>
            {
                @object = new SNReportingDataDTO
                              {
                                  discussion = data == null ? null : new SNGroupDiscussionDTO(data),
                                  members = SNMemberModel.GetAllByACSessionId(sessionId).Select(x => new SNMemberDTO(x)).ToList()
                              }
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ReportDTO> GetSplashScreenReports(int userId, int pageIndex, int pageSize)
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SplashScreenDTO> GetSplashScreen(int userId)
        {
            return new ServiceResponse<SplashScreenDTO>
                       {
                           @object =
                               new SplashScreenDTO
                                   {
                                       recentReports = this.GetRecentSplashScreenReports(userId, 1, 10).objects.ToList(),
                                       reports = this.GetSplashScreenReports(userId, 1, 5).objects.ToList()
                                   }
                       };
        }

        #endregion
    }
}