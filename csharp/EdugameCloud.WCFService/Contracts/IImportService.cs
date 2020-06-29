namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The import service interface.
    /// </summary>
    [ServiceContract]
    public interface IImportService
    {
        /// <summary>
        /// Imports questions by subModule id and file id.
        /// </summary>
        /// <param name="subModuleId">SubModule id.</param>
        /// <param name="fileId">File id.</param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void ImportQuestionsByFileId(int subModuleId, string fileId);
    }
}
