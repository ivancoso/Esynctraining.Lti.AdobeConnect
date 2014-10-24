namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The LmsService interface.
    /// </summary>
    [ServiceContract]
    public interface ILmsService
    {
        /// <summary>
        /// The get quizzes for user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<LmsQuizDTO> GetQuizzesForUser(int userId, int lmsUserParametersId);

        /// <summary>
        /// The get authentication parameters by id.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<LmsUserParametersDTO> GetAuthenticationParametersById(LmsAuthenticationParametersDTO parameters);

        /// <summary>
        /// The convert quizzes.
        /// </summary>
        /// <param name="quizzesInfo">
        /// The quizzes info.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<QuizesAndSubModuleItemsDTO> ConvertQuizzes(LmsQuizConvertDTO quizzesInfo);
    }
}
