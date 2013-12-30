// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Domain.Formats.Edugame;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
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
        /// Gets the sub module category model.
        /// </summary>
        private SubModuleCategoryModel SubModuleCategoryModel
        {
            get
            {
                return IoC.Resolve<SubModuleCategoryModel>();
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
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<TestDTO> GetAll()
        {
            return new ServiceResponse<TestDTO> { objects = this.TestModel.GetAll().Select(x => new TestDTO(x)).ToList() };
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<TestDTO>
            {
                objects = this.TestModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new TestDTO(x)).ToList(),
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestDTO> Create(TestSMIWrapperDTO dto)
        {
            var result = new ServiceResponse<TestDTO>();
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var quizModel = this.TestModel;
                var smiResult = this.ConvertDto(dto.SmiDTO, null);
                this.SubModuleItemModel.RegisterSave(smiResult, true);
                dto.TestDTO.subModuleItemId = smiResult.Id;
                return this.ConvertTestAndGetServiceResponse(dto.TestDTO, null, quizModel, result);
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestDTO> Save(TestDTO appletResultDTO)
        {
            var result = new ServiceResponse<TestDTO>();
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizModel = this.TestModel;
                var isTransient = appletResultDTO.testId == 0;
                var quiz = isTransient ? null : quizModel.GetOneById(appletResultDTO.testId).Value;
                return this.ConvertTestAndGetServiceResponse(appletResultDTO, quiz, quizModel, result);
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestDTO> GetById(int id)
        {
            var result = new ServiceResponse<TestDTO>();
            Test test;
            if ((test = this.TestModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new TestDTO(test);
            }

            return result;
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestDTO> GetBySMIId(int id)
        {
            var result = new ServiceResponse<TestDTO>();
            Test test;
            if ((test = this.TestModel.GetOneBySMIId(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new TestDTO(test);
            }

            return result;
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestFromStoredProcedureDTO> GetTestsByUserId(int userId)
        {
            return new ServiceResponse<TestFromStoredProcedureDTO> { objects = this.TestModel.GetTestsByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestFromStoredProcedureDTO> GetSharedTestsByUserId(int userId)
        {
            return new ServiceResponse<TestFromStoredProcedureDTO> { objects = this.TestModel.GetSharedForUserTestsByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SMICategoriesFromStoredProcedureDTO> GetTestCategoriesbyUserId(int userId)
        {
            return new ServiceResponse<SMICategoriesFromStoredProcedureDTO> { objects = this.TestModel.GetTestCategoriesbyUserId(userId).ToList() };
        }

        /// <summary>
        /// The get test sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO> GetTestSMItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTOFromStoredProcedureDTO> { objects = this.TestModel.GetTestSMItemsByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get test data by test id.
        /// </summary>
        /// <param name="testId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<TestDataDTO> GetTestDataByTestId(int testId)
        {
            return new ServiceResponse<TestDataDTO> { @object = this.TestModel.GetTestDataByTestId(testId) };
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
        /// The convert DTO.
        /// </summary>
        /// <param name="smi">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        private SubModuleItem ConvertDto(SubModuleItemDTO smi, SubModuleItem instance)
        {
            instance = instance ?? new SubModuleItem();
            instance.IsActive = smi.isActive;
            instance.IsShared = smi.isShared;
            instance.DateCreated = smi.dateCreated == DateTime.MinValue ? DateTime.Now : smi.dateCreated;
            instance.DateModified = smi.dateModified == DateTime.MinValue ? DateTime.Now : smi.dateModified;
            instance.SubModuleCategory = this.SubModuleCategoryModel.GetOneById(smi.subModuleCategoryId).Value;
            instance.CreatedBy = smi.createdBy.HasValue ? this.UserModel.GetOneById(smi.createdBy.Value).Value : null;
            instance.ModifiedBy = smi.modifiedBy.HasValue ? this.UserModel.GetOneById(smi.modifiedBy.Value).Value : null;
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
        /// <param name="testModel">
        /// The test model.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse<TestDTO> ConvertTestAndGetServiceResponse(TestDTO appletResultDTO, Test test, TestModel testModel, ServiceResponse<TestDTO> result)
        {
            test = this.ConvertDto(appletResultDTO, test);
            testModel.RegisterSave(test, true);
            int companyId = test.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Test>(NotificationType.Update, companyId, test.Id);
            result.@object = new TestDTO(test);
            return result;
        }

        #endregion
    }
}