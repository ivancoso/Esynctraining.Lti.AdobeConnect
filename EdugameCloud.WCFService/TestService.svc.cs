namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class TestService : BaseService, ITestService
    {
        #region Properties

        /// <summary>
        /// Gets the Test model.
        /// </summary>
        private TestModel TestModel
        {
            get
            {
                return IoC.Resolve<TestModel>();
            }
        }

        /// <summary>
        /// Gets the Score model.
        /// </summary>
        private ScoreTypeModel ScoreTypeModel
        {
            get
            {
                return IoC.Resolve<ScoreTypeModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All items test.
        /// </summary>
        /// <returns>
        ///     The <see cref="TestDTO" />.
        /// </returns>
        public TestDTO[] GetAll()
        {
            return this.TestModel.GetAll().Select(x => new TestDTO(x)).ToArray();
        }

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
        /// The <see cref="PagedTestDTO"/>.
        /// </returns>
        public PagedTestDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedTestDTO
            {
                objects = this.TestModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new TestDTO(x)).ToArray(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The creation of quiz.
        /// </summary>
        /// <param name="dto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        public TestWithSmiDTO Create(TestSMIWrapperDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var quizModel = this.TestModel;
                var smiResult = this.Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.TestDTO.subModuleItemId = smiResult.Id;
                return this.ConvertTestAndGetServiceResponse(dto.TestDTO, null, smiResult, quizModel);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Test.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        public TestWithSmiDTO Save(TestDTO appletResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizModel = this.TestModel;
                var isTransient = appletResultDTO.testId == 0;
                var quiz = isTransient ? null : quizModel.GetOneById(appletResultDTO.testId).Value;
                return this.ConvertTestAndGetServiceResponse(appletResultDTO, quiz, null, quizModel);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Test.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        public TestWithSmiDTO GetById(int id)
        {
            Test test;
            if ((test = this.TestModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Test.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new TestWithSmiDTO(test);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        public TestWithSmiDTO GetBySMIId(int id)
        {
            Test test;
            if ((test = this.TestModel.GetOneBySMIId(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Test.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new TestWithSmiDTO(test);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TestFromStoredProcedureDTO"/>.
        /// </returns>
        public TestFromStoredProcedureDTO[] GetTestsByUserId(int userId)
        {
            return this.TestModel.GetTestsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TestFromStoredProcedureDTO"/>.
        /// </returns>
        public TestFromStoredProcedureDTO[] GetSharedTestsByUserId(int userId)
        {
            return this.TestModel.GetSharedForUserTestsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SMICategoriesFromStoredProcedureDTO"/>.
        /// </returns>
        public SMICategoriesFromStoredProcedureDTO[] GetTestCategoriesbyUserId(int userId)
        {
            return this.TestModel.GetTestCategoriesbyUserId(userId).ToArray();
        }

        /// <summary>
        /// The get test sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetTestSMItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetTestSubModuleItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get test data by test id.
        /// </summary>
        /// <param name="testId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="TestDataDTO"/>.
        /// </returns>
        public TestDataDTO GetTestDataByTestId(int testId)
        {
            return this.TestModel.GetTestDataByTestId(testId);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="itemDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="Test"/>.
        /// </returns>
        private Test ConvertDto(TestDTO itemDTO, Test instance)
        {
            instance = instance ?? new Test();
            instance.TestName = itemDTO.testName;
            instance.Description = itemDTO.description;
            instance.InstructionTitle = itemDTO.instructionTitle;
            instance.InstructionDescription = itemDTO.instructionDescription;
            instance.PassingScore = itemDTO.passingScore;
            instance.ScoreFormat = itemDTO.scoreFormat;
            instance.TimeLimit = itemDTO.timeLimit;

            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
            instance.ScoreType = itemDTO.scoreTypeId.HasValue ? this.ScoreTypeModel.GetOneById(itemDTO.scoreTypeId.Value).Value ?? this.ScoreTypeModel.GetOneById(1).Value : this.ScoreTypeModel.GetOneById(1).Value;
            if (instance.SubModuleItem != null)
            {
                instance.SubModuleItem.DateModified = DateTime.Now;
                this.SubModuleItemModel.RegisterSave(instance.SubModuleItem);
            }

            return instance;
        }

        /// <summary>
        /// The convert test and get service response.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The test result DTO.
        /// </param>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <param name="smi">
        /// The SMI.
        /// </param>
        /// <param name="testModel">
        /// The test model.
        /// </param>
        /// <returns>
        /// The <see cref="TestWithSmiDTO"/>.
        /// </returns>
        private TestWithSmiDTO ConvertTestAndGetServiceResponse(TestDTO appletResultDTO, Test test, SubModuleItem smi, TestModel testModel)
        {
            test = this.ConvertDto(appletResultDTO, test);
            testModel.RegisterSave(test, true);
            int companyId = smi.With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Test>(NotificationType.Update, companyId, test.Id);
            return new TestWithSmiDTO(test, smi);
        }

        #endregion
    }
}