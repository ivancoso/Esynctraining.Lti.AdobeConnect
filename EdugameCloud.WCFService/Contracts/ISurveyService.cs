namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Survey Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISurveyService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO GetBySMIId(int smiId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SMICategoriesFromStoredProcedureDTO[] GetSurveyCategoriesbyUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDataDTO GetSurveyDataBySurveyId(int surveyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SubModuleItemDTO[] GetSurveySMItemsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyFromStoredProcedureDTO[] GetSurveysByUserId(int userId, bool? showLms);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyFromStoredProcedureDTO[] GetSharedSurveysByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyFromStoredProcedureDTO[] GetLmsSurveys(int userId, int lmsUserParametersId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO Save(SurveyDTO surveyDTO);

        [OperationContract]
        [FaultContract(typeof(Error))]
        SurveyDTO Create(SurveySMIWrapperDTO dto);

    }

}