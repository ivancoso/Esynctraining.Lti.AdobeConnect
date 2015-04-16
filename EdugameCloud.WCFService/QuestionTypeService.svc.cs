﻿namespace EdugameCloud.WCFService
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
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        public QuestionTypeDTO[] GetAll()
        {
            return this.QuestionTypeModel.GetAll().ToList().Select(x => new QuestionTypeDTO(x)).ToArray();
        }

        /// <summary>
        /// The get active types.
        /// </summary>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        public QuestionTypeDTO[] GetActiveTypes()
        {
            return this.QuestionTypeModel.GetAllActive().ToList().Select(x => new QuestionTypeDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
        public QuestionTypeDTO Save(QuestionTypeDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var typeModel = this.QuestionTypeModel;
                var isTransient = resultDto.questionTypeId == 0;
                var convertDto = isTransient ? null : typeModel.GetOneById(resultDto.questionTypeId).Value;
                convertDto = this.ConvertDto(resultDto, convertDto);
                typeModel.RegisterSave(convertDto);
                return new QuestionTypeDTO(convertDto);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("QuestionType.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuestionTypeDTO"/>.
        /// </returns>
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

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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
                this.LogError("QuestionType.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(entity, true);
            return id;
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
        /// The <see cref="PagedQuestionTypeDTO"/>.
        /// </returns>
        public PagedQuestionTypeDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedQuestionTypeDTO
            {
                objects = this.QuestionTypeModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new QuestionTypeDTO(x)).ToArray(),
                totalCount = totalCount
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="q">
        /// The result DTO.
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