using Esynctraining.Core.Domain.Entities;

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
        /// Array of TopicDTO.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TopicDTO[] GetAllByCategoryId(int categoryId);

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// Array of TopicDTO.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TopicDTO GetById(int id);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// Array of TopicDTO.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TopicDTO[] SaveAll(TopicDTO[] results);

        /// <summary>
        /// The registration.
        /// </summary>
        /// <param name="topicDTO">
        /// The participant DTO.
        /// </param>
        /// <returns>
        /// TopicDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TopicDTO Save(TopicDTO topicDTO);

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The id
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// The get all by category or company and name paged.
        /// </summary>
        /// <param name="searchPattern">
        /// The search pattern.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The array of TopicDTO.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TopicPageWrapper GetAllByCategoryAndNamePaged(
            string searchPattern,
            int categoryId,
            int pageIndex,
            int pageSize);

        #endregion
    }
}