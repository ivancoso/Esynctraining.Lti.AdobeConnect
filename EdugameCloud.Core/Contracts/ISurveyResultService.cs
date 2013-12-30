namespace EdugameCloud.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The SurveyResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISurveyResultService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyResultDTO> GetAll();

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyResultDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="surveyResultDTO">
        /// The survey result.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyResultDTO> Save(SurveyResultDTO surveyResultDTO);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="results">
        /// The survey results.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SurveyResultDTO> SaveAll(List<SurveyResultDTO> results);

        #endregion
    }
}