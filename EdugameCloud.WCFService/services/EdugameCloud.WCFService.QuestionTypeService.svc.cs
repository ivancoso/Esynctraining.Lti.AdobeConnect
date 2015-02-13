// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuestionTypeService : BaseService, IQuestionTypeService
    {
        #region Properties

        /// <summary>
        /// Gets the question type model.
        /// </summary>
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionTypeDTO> GetAll()
        {
            return new ServiceResponse<QuestionTypeDTO> { objects = this.QuestionTypeModel.GetAll().ToList().Select(x => new QuestionTypeDTO(x)).ToList() };
        }

        /// <summary>
        /// The get active types.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionTypeDTO> GetActiveTypes()
        {
            return new ServiceResponse<QuestionTypeDTO> { objects = this.QuestionTypeModel.GetAllActive().ToList().Select(x => new QuestionTypeDTO(x)).ToList() };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuestionTypeDTO> Save(QuestionTypeDTO resultDto)
        {
            var result = new ServiceResponse<QuestionTypeDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var typeModel = this.QuestionTypeModel;
                var isTransient = resultDto.questionTypeId == 0;
                var convertDto = isTransient ? null : typeModel.GetOneById(resultDto.questionTypeId).Value;
                convertDto = this.ConvertDto(resultDto, convertDto);
                typeModel.RegisterSave(convertDto);
                result.@object = new QuestionTypeDTO(convertDto);
                return result;
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
        public ServiceResponse<QuestionTypeDTO> GetById(int id)
        {
            var result = new ServiceResponse<QuestionTypeDTO>();
            QuestionType questionType;
            if ((questionType = this.QuestionTypeModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new QuestionTypeDTO(questionType);
            }

            return result;
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            QuestionType entity;
            var model = this.QuestionTypeModel;
            if ((entity = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(entity, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
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
        public ServiceResponse<QuestionTypeDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<QuestionTypeDTO>
            {
                objects = this.QuestionTypeModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new QuestionTypeDTO(x)).ToList(),
                totalCount = totalCount
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert dto.
        /// </summary>
        /// <param name="q">
        /// The result dto.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionType"/>.
        /// </returns>
        private QuestionType ConvertDto(QuestionTypeDTO q, QuestionType instance)
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