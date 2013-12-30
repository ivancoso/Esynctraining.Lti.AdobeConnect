namespace EdugameCloud.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The AppletResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface IAppletResultService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<AppletResultDTO> GetAll();

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
        ServiceResponse<AppletResultDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<AppletResultDTO> Save(AppletResultDTO appletResultDTO);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="appletResultDTOs">
        /// The applet result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        // ReSharper disable once InconsistentNaming
        ServiceResponse<AppletResultDTO> SaveAll(List<AppletResultDTO> appletResultDTOs);
        
        #endregion
    }
}