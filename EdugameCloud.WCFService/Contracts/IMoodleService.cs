namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The MoodleService interface.
    /// </summary>
    [ServiceContract]
    public interface IMoodleService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="userInfo">
        /// The user info
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        /// <summary>
        /// The get quizes.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<MoodleQuizInfoDTO> GetQuizzesForUser(MoodleUserInfoDTO userInfo);

        /// <summary>
        /// The get surveys for user
        /// </summary>
        /// <param name="userInfo">
        /// The user info
        /// </param>
        /// <returns></returns>
        [OperationContract]
        ServiceResponse<MoodleQuizInfoDTO> GetSurveysForUser(MoodleUserInfoDTO userInfo);

        /// <summary>
        /// The convert quizes.
        /// </summary>
        /// <param name="quiz">
        /// The quiz
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizesAndSubModuleItemsDTO> ConvertQuizzes(LmsQuizConvertDTO quiz);

        /// <summary>
        /// The convert surveys.
        /// </summary>
        /// <param name="survey">
        /// The survey
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveysAndSubModuleItemsDTO> ConvertSurveys(LmsQuizConvertDTO survey);

        /// <summary>
        /// The get authentication parameters by id
        /// </summary>
        /// <param name="id">
        /// The id
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<LmsUserParametersDTO> GetAuthenticationParametersById(string id);

        [OperationContract]
        void Test();

        #endregion
    }
}
