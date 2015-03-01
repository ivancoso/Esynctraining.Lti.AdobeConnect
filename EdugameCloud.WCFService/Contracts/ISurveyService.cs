namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

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
        ///     The <see cref="SurveyDTO" />.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO[] GetAll();

        /// <summary>
        /// Get user by subModuleId.
        /// </summary>
        /// <param name="id">
        /// The subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO GetById(int id);

        /// <summary>
        /// Get user by sub Module Id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO GetBySMIId(int smiId);

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
        /// The <see cref="PagedSurveyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        PagedSurveyDTO GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The get survey categories by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SMICategoriesFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SMICategoriesFromStoredProcedureDTO[] GetSurveyCategoriesbyUserId(int userId);

        /// <summary>
        /// The get survey data by quiz subModuleId.
        /// </summary>
        /// <param name="surveyId">
        /// The survey subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDataDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDataDTO GetSurveyDataBySurveyId(int surveyId);

        /// <summary>
        /// The get survey SM items by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SubModuleItemDTO[] GetSurveySMItemsByUserId(int userId);

        /// <summary>
        /// The get surveys by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <param name="showLms">
        /// The show LMS
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyFromStoredProcedureDTO[] GetSurveysByUserId(int userId, bool? showLms);

        /// <summary>
        /// The get shared surveys by user subModuleId.
        /// </summary>
        /// <param name="userId">
        /// The user subModuleId.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyFromStoredProcedureDTO[] GetSharedSurveysByUserId(int userId);

        /// <summary>
        /// The get LMS surveys.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyFromStoredProcedureDTO[] GetLmsSurveys(int userId, int lmsUserParametersId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="surveyDTO">
        /// The survey.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO Save(SurveyDTO surveyDTO);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO Create(SurveySMIWrapperDTO dto);

        #endregion
    }
}