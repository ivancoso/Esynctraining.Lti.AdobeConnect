namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface IDistractorService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO[] GetAllByUserIdAndSubModuleItemId(int userId, int smiId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO[] GetAllByUserIdAndQuestionId(int userId, int questionId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        DistractorDTO Save(DistractorDTO resultDto);

    }

}