namespace EdugameCloud.WCFService
{
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
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuestionTypeService : BaseService, IQuestionTypeService
    {
        private QuestionTypeModel QuestionTypeModel => IoC.Resolve<QuestionTypeModel>();

        #region Public Methods and Operators

        public QuestionTypeDTO[] GetAll()
        {
            return this.QuestionTypeModel.GetAll().ToList().Select(x => new QuestionTypeDTO(x)).ToArray();
        }

        public QuestionTypeDTO[] GetActiveTypes()
        {
            return this.QuestionTypeModel.GetAllActive().ToList().Select(x => new QuestionTypeDTO(x)).ToArray();
        }

        public QuestionTypeDTO Save(QuestionTypeDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var typeModel = this.QuestionTypeModel;
                var isTransient = resultDto.questionTypeId == 0;
                var convertDto = isTransient ? null : typeModel.GetOneById(resultDto.questionTypeId).Value;
                convertDto = ConvertDto(resultDto, convertDto);
                typeModel.RegisterSave(convertDto);
                return new QuestionTypeDTO(convertDto);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("QuestionType.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public QuestionTypeDTO GetById(int id)
        {
            QuestionType questionType;
            if ((questionType = this.QuestionTypeModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuestionType.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuestionTypeDTO(questionType);
        }

        public int DeleteById(int id)
        {
            QuestionType entity;
            var model = this.QuestionTypeModel;
            if ((entity = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                LogError("QuestionType.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(entity, true);
            return id;
        }

        #endregion

        #region Methods

        private static QuestionType ConvertDto(QuestionTypeDTO q, QuestionType instance)
        {
            instance = instance ?? new QuestionType();
            instance.IsActive = q.isActive;
            instance.Type = q.type;
            instance.QuestionTypeOrder = q.questionTypeOrder;
            instance.QuestionTypeDescription = q.questionTypeDescription;
            instance.Instruction = q.instruction;
            instance.CorrectText = q.correctText;
            instance.IncorrectMessage = q.incorrectMessage;
            instance.IconSource = q.iconSource;

            return instance;
        }

        #endregion
    }
}