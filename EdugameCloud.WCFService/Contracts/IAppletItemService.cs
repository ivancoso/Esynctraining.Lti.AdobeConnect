namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface IAppletItemService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO[] GetByUser(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO GetBySMIId(int smiId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CrosswordDTO[] GetCrosswordsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CrosswordDTO[] GetSharedCrosswordsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        AppletItemDTO Save(AppletItemDTO appletItemDTO);

    }

}