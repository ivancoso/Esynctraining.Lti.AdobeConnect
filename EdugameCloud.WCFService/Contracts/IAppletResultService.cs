namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

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
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletResultDTO[] GetAll();

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletResultDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletResultDTO Save(AppletResultDTO appletResultDTO);

        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="appletResultDTOs">
        /// The applet result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="AppletResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        // ReSharper disable once InconsistentNaming
        AppletResultSaveAllDTO SaveAll(AppletResultDTO[] appletResultDTOs);
        
        #endregion
    }
}