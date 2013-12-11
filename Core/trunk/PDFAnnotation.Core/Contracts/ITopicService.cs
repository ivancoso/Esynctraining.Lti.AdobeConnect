namespace PDFAnnotation.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The Topic interface.
    /// </summary>
    [ServiceContract]
    public interface ITopicService
    {
        #region Public Methods and Operators

        /// <summary>
        /// All topics test.
        /// </summary>
        /// <param name="categoryId">
        /// The case Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TopicDTO> GetAllByCategoryId(int categoryId);

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TopicDTO> GetById(int id);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TopicDTO> SaveAll(List<TopicDTO> results);

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="topicDTO">
        /// The participant DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TopicDTO> Save(TopicDTO topicDTO);

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteById(int id);

        #endregion
    }
}