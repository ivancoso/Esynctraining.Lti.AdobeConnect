namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The AppletItem Service interface.
    /// </summary>
    [ServiceContract]
    public interface IAppletItemService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO[] GetAll();

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO GetById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO GetBySMIId(int smiId);

        /// <summary>
        /// The get crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CrosswordDTO[] GetCrosswordsByUserId(int userId);

        /// <summary>
        /// The get shared crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="CrosswordDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CrosswordDTO[] GetSharedCrosswordsByUserId(int userId);

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
        /// The <see cref="PagedAppletItemsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        PagedAppletItemsDTO GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletItemDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="AppletItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO Save(AppletItemDTO appletItemDTO);

        #endregion
    }
}