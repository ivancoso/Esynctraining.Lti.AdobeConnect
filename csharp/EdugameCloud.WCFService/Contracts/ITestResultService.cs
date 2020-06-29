namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface ITestResultService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        Task SaveAll(TestSummaryResultDTO testResult);

    }

}