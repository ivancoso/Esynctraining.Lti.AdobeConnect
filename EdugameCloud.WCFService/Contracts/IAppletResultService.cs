namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The AppletResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface IAppletResultService
    {
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

    }

}