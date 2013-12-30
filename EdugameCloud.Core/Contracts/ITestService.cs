namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The Test Service interface.
    /// </summary>
    [ServiceContract]
    public interface ITestService
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The all.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDTO> GetAll();

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDTO> GetById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDTO> GetBySMIId(int smiId);

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDTO> GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SMICategoriesFromStoredProcedureDTO> GetTestCategoriesbyUserId(int userId);

        /// <summary>
        /// The get quiz data by test id.
        /// </summary>
        /// <param name="testId">
        /// The test id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDataDTO> GetTestDataByTestId(int testId);

        /// <summary>
        /// The get test SM items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO> GetTestSMItemsByUserId(int userId);

        /// <summary>
        /// The get tests by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestFromStoredProcedureDTO> GetTestsByUserId(int userId);

        /// <summary>
        /// The get shared tests by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestFromStoredProcedureDTO> GetSharedTestsByUserId(int userId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="appletItemDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDTO> Save(TestDTO appletItemDTO);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<TestDTO> Create(TestSMIWrapperDTO dto);

        #endregion
    }
}