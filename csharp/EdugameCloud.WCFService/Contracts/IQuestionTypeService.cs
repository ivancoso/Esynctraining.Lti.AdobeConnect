namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface IQuestionTypeService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO[] GetActiveTypes();

        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionTypeDTO Save(QuestionTypeDTO user);

    }

}