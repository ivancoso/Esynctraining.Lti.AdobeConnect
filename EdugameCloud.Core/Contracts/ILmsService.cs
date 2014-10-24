﻿namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

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
        ServiceResponse<LmsQuizInfoDTO> GetQuizzesForUser(int userId, int lmsUserParametersId);

        /// <summary>
        /// The get authentication parameters by id.
        /// </summary>
        /// <param name="acId">
        /// The ac id.
        /// </param>
        /// <param name="acDomain">
        /// The ac domain.
        /// </param>
        /// <param name="scoId">
        /// The sco id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<LmsUserParametersDTO> GetAuthenticationParametersById(string acId, string acDomain, string scoId);

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
