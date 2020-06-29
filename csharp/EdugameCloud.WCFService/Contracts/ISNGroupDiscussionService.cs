namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface ISNGroupDiscussionService 
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNGroupDiscussionDTO Save(SNGroupDiscussionDTO discussion);

    }

}