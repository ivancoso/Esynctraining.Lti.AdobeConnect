namespace EdugameCloud.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<AppletItemDTO> GetAll();

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
        ServiceResponse<AppletItemDTO> GetById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="smiId">
        /// The smi Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<AppletItemDTO> GetBySMIId(int smiId);

        /// <summary>
        /// The get crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CrosswordDTO> GetCrosswordsByUserId(int userId);

        /// <summary>
        /// The get shared crosswords by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CrosswordDTO> GetSharedCrosswordsByUserId(int userId);

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
        ServiceResponse<AppletItemDTO> GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletItemDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<AppletItemDTO> Save(AppletItemDTO appletItemDTO);

        #endregion
    }
}