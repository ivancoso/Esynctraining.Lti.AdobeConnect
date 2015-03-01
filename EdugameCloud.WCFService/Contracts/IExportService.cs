namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The export service interface.
    /// </summary>
    [ServiceContract]
    public interface IExportService
    {
        /// <summary>
        /// Export questions by SubModule id.
        /// </summary>
        /// <param name="questionsModel">
        /// The questions Model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string ExportQuestionsBySubModuleItemId(bool questionsModel);

        /// <summary>
        /// Export by SubModule id.
        /// </summary>
        /// <param name="subModuleId">SubModule id.</param>
        /// <param name="format">Export format.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string ExportBySubModuleId(int subModuleId, string format);
    }
}
