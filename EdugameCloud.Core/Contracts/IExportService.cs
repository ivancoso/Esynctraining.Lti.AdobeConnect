namespace EdugameCloud.Core.Contracts
{
    using Esynctraining.Core.Domain.Contracts;
    using System.ServiceModel;
    
    /// <summary>
    /// The export service interface.
    /// </summary>
    [ServiceContract]
    public interface IExportService
    {
        /// <summary>
        /// Export questions by SubModule id.
        /// </summary>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse<string> ExportQuestionsBySubModuleItemId(bool questionsModel);

        /// <summary>
        /// Export by SubModule id.
        /// </summary>
        /// <param name="subModuleId">SubModule id.</param>
        /// <param name="format">Export format.</param>
        /// <returns>The <see cref="ServiceResponse"/>.</returns>
        [OperationContract]
        ServiceResponse<string> ExportBySubModuleId(int subModuleId, string format);
    }
}
