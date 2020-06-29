namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class TestService : BaseService, ITestService
    {
        #region Properties

        private TestModel TestModel => IoC.Resolve<TestModel>();

        private ScoreTypeModel ScoreTypeModel => IoC.Resolve<ScoreTypeModel>();

        #endregion

        #region Public Methods and Operators
        
        public TestWithSmiDTO Create(TestSMIWrapperDTO dto)
        {
            if (this.IsValid(dto, out ValidationResult validationResult))
            {
                var smiResult = Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.TestDTO.subModuleItemId = smiResult.Id;
                return ConvertTestAndGetServiceResponse(dto.TestDTO, null, smiResult, TestModel);
            }

            var error = GenerateValidationError(validationResult);
            LogError("Test.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public TestWithSmiDTO Save(TestDTO appletResultDTO)
        {
            if (IsValid(appletResultDTO, out ValidationResult validationResult))
            {
                var model = TestModel;
                var isTransient = appletResultDTO.testId == 0;
                var quiz = isTransient ? null : model.GetOneById(appletResultDTO.testId).Value;
                return ConvertTestAndGetServiceResponse(appletResultDTO, quiz, null, model);
            }

            var error = GenerateValidationError(validationResult);
            LogError("Test.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public TestWithSmiDTO GetById(int id)
        {
            Test test;
            if ((test = TestModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                LogError("Test.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new TestWithSmiDTO(test);
        }

        public TestWithSmiDTO GetBySMIId(int id)
        {
            Test test;
            if ((test = TestModel.GetOneBySMIId(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                LogError("Test.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new TestWithSmiDTO(test);
        }

        public TestFromStoredProcedureDTO[] GetTestsByUserId(int userId)
        {
            return TestModel.GetTestsByUserId(userId).ToArray();
        }

        public TestFromStoredProcedureDTO[] GetSharedTestsByUserId(int userId)
        {
            return TestModel.GetSharedForUserTestsByUserId(userId).ToArray();
        }

        public SMICategoriesFromStoredProcedureDTO[] GetTestCategoriesbyUserId(int userId)
        {
            return TestModel.GetTestCategoriesbyUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetTestSMItemsByUserId(int userId)
        {
            return SubModuleItemModel.GetTestSubModuleItemsByUserId(userId).ToArray();
        }

        public TestDataDTO GetTestDataByTestId(int testId)
        {
            return TestModel.GetTestDataByTestId(testId);
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

            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
            instance.ScoreType = itemDTO.scoreTypeId.HasValue ? ScoreTypeModel.GetOneById(itemDTO.scoreTypeId.Value).Value ?? ScoreTypeModel.GetOneById(1).Value : this.ScoreTypeModel.GetOneById(1).Value;
            if (instance.SubModuleItem != null)
            {
                instance.SubModuleItem.DateModified = DateTime.Now;
                SubModuleItemModel.RegisterSave(instance.SubModuleItem);
            }

            return instance;
        }

        private TestWithSmiDTO ConvertTestAndGetServiceResponse(TestDTO appletResultDTO, Test test, SubModuleItem smi, TestModel testModel)
        {
            test = ConvertDto(appletResultDTO, test);
            testModel.RegisterSave(test, true);
            return new TestWithSmiDTO(test, smi);
        }

        #endregion
    }
}