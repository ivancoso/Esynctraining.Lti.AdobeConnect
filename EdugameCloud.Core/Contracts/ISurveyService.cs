namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Formats.Edugame;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The Survey Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISurveyService
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The all.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyDTO> GetAll();

        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="id">
        /// The subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyDTO> GetById(int id);

        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="smiId">
        /// The smi Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyDTO> GetBySMIId(int smiId);

        /// <summary>
        /// The get paged.
        /// </summary>
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
        ServiceResponse<SurveyDTO> GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The get survey categoriesby user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SMICategoriesFromStoredProcedureDTO> GetSurveyCategoriesbyUserId(int userId);

        /// <summary>
        /// The get survey data by quiz subModuleId.
        /// </summary>
        /// <param name="surveyId">
        /// The survey subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyDataDTO> GetSurveyDataBySurveyId(int surveyId);

        /// <summary>
        /// The get survey sm items by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubModuleItemDTO> GetSurveySMItemsByUserId(int userId);

        /// <summary>
        /// The get surveys by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <param name="showLms">
        /// The show lms
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyFromStoredProcedureDTO> GetSurveysByUserId(int userId, bool? showLms);

        /// <summary>
        /// The get shared surveys by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyFromStoredProcedureDTO> GetSharedSurveysByUserId(int userId);

        /// <summary>
        /// The get lms surveys.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyFromStoredProcedureDTO> GetLmsSurveys(int userId, int lmsUserParametersId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="surveyDTO">
        /// The survey.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyDTO> Save(SurveyDTO surveyDTO);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyDTO> Create(SurveySMIWrapperDTO dto);

        #endregion
    }
}