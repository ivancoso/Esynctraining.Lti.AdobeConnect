namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Test Service interface.
    /// </summary>
    [ServiceContract]
    public interface ITestService
    {
        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestWithSmiDTO GetById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestWithSmiDTO GetBySMIId(int smiId);

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SMICategoriesFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SMICategoriesFromStoredProcedureDTO[] GetTestCategoriesbyUserId(int userId);

        /// <summary>
        /// The get quiz data by test id.
        /// </summary>
        /// <param name="testId">
        /// The test id.
        /// </param>
        /// <returns>
        /// The <see cref="TestDataDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestDataDTO GetTestDataByTestId(int testId);

        /// <summary>
        /// The get test SM items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SubModuleItemDTO[] GetTestSMItemsByUserId(int userId);

        /// <summary>
        /// The get tests by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="TestFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestFromStoredProcedureDTO[] GetTestsByUserId(int userId);

        /// <summary>
        /// The get shared tests by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="TestFromStoredProcedureDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestFromStoredProcedureDTO[] GetSharedTestsByUserId(int userId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletItemDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestWithSmiDTO Save(TestDTO appletItemDTO);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        TestWithSmiDTO Create(TestSMIWrapperDTO dto);

    }

}