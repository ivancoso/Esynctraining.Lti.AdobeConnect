namespace EdugameCloud.Core.Contracts
{
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The Reporting Service interface.
    /// </summary>
    [ServiceContract]
    public interface IReportingService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get crossword result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here."), OperationContract]
        ServiceResponse<CrosswordResultByAcSessionDTO> GetCrosswordResultByACSessionId(int acSessionId);

        /// <summary>
        /// The get crossword sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CrosswordSessionDTO> GetCrosswordSessionsByUserId(int userId);

        /// <summary>
        /// The get quiz result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed here"), OperationContract]
        ServiceResponse<QuizResultDataDTO> GetQuizResultByACSessionId(int acSessionId, int smiId);

        /// <summary>
        /// The get survey result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed here"), OperationContract]
        ServiceResponse<SurveyResultDataDTO> GetSurveyResultByACSessionId(int acSessionId, int smiId);

        /// <summary>
        /// The get survey result by AC session id.
        /// </summary>
        /// <param name="acSessionId">
        /// The AC session id.
        /// </param>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed here"), OperationContract]
        ServiceResponse<TestResultDataDTO> GetTestResultByACSessionId(int acSessionId, int smiId);

        /// <summary>
        /// The get quiz sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizSessionDTO> GetQuizSessionsByUserId(int userId);

        /// <summary>
        /// The get survey sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveySessionDTO> GetSurveySessionsByUserId(int userId);

        /// <summary>
        /// The get test sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestSessionDTO> GetTestSessionsByUserId(int userId);

        /// <summary>
        /// The get recent splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user id.
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
        [OperationContract]
        ServiceResponse<RecentReportDTO> GetRecentSplashScreenReports(int userId, int pageIndex, int pageSize);

        /// <summary>
        /// The get SN data by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The AC session id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNReportingDataDTO> GetSNDataByACSessionId(int sessionId);

        /// <summary>
        /// The get SN sessions by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNSessionDTO> GetSNSessionsByUserId(int userId);

        /// <summary>
        /// The get splash screen reports.
        /// </summary>
        /// <param name="userId">
        /// The user id.
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
        [OperationContract]
        ServiceResponse<ReportDTO> GetSplashScreenReports(int userId, int pageIndex, int pageSize);

        /// <summary>
        /// The get splash screen.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SplashScreenDTO> GetSplashScreen(int userId);

        #endregion
    }
}