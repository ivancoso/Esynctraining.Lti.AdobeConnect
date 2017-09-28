namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The TestResult Service interface.
    /// </summary>
    [ServiceContract]
    public interface ITestResultService
    {
        /// <summary>
        /// The save all.
        /// </summary>
        /// <param name="results">
        /// The applet result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="TestResultDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestResultSaveAllDTO SaveAll(TestSummaryResultDTO testResult);

    }

}