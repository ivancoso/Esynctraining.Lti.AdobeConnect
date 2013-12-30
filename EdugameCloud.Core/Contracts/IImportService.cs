namespace EdugameCloud.Core.Contracts
{
    using Esynctraining.Core.Domain.Contracts;
    using System.ServiceModel;

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
        /// <returns>The <see cref="ServiceResponse{T}"/>.</returns>
        [OperationContract]
        ServiceResponse ImportQuestionsByFileId(int subModuleId, string fileId);
    }
}
