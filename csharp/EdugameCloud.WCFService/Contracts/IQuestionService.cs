namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///  The question Service interface.
    /// </summary>
    [ServiceContract]
    public interface IQuestionService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO[] GetByUserIdAndSubModuleItemId(int userId, int smiId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO Save(QuestionDTO dto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionSaveAllDTO SaveAll(QuestionDTO[] questions);

        [OperationContract]
        [FaultContract(typeof(Error))]
        void Reorder(QuestionOrderDTO[] questions);

        [OperationContract]
        [FaultContract(typeof(Error))]
        string ExportQuestionsBySubModuleItemId(int smiId, int[] questionIds);

        [OperationContract]
        [FaultContract(typeof(Error))]
        string ExportBySubModuleId(int subModuleId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO[] ImportQuestionsBySubModuleItemId(string id, int smiId, string format);

        [OperationContract]
        [FaultContract(typeof(Error))]
        QuestionDTO[] GetParsedQuestionsById(string id, int userId, string format);

    }

}