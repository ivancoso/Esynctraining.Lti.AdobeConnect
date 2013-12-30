// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Collections.Generic;
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
    public class QuizQuestionResultService : BaseService, IQuizQuestionResultService
    {
        #region Properties

        /// <summary>
        /// Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        /// <summary>
        /// Gets the quiz question result model.
        /// </summary>
        private QuizQuestionResultModel QuizQuestionResultModel
        {
            get
            {
                return IoC.Resolve<QuizQuestionResultModel>();
            }
        }

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

        /// <summary>
        /// Gets the question model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Gets all quiz questions results.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<QuizQuestionResultDTO> GetAll()
        {
            return new ServiceResponse<QuizQuestionResultDTO> { objects = this.QuizQuestionResultModel.GetAll().Select(x => new QuizQuestionResultDTO(x)).ToList() };
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
        public ServiceResponse<QuizQuestionResultDTO> Save(QuizQuestionResultDTO resultDto)
        {
            var result = new ServiceResponse<QuizQuestionResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var quizQuestionResultModel = this.QuizQuestionResultModel;
                var isTransient = resultDto.quizQuestionResultId == 0;
                var quizQuestionResult = isTransient ? null : quizQuestionResultModel.GetOneById(resultDto.quizQuestionResultId).Value;
                quizQuestionResult = this.ConvertDto(resultDto, quizQuestionResult);
                quizQuestionResultModel.RegisterSave(quizQuestionResult);
                result.@object = new QuizQuestionResultDTO(quizQuestionResult);
                return result;
            }

            result = (ServiceResponse<QuizQuestionResultDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizQuestionResultDTO> SaveAll(List<QuizQuestionResultDTO> results)
        {
            var result = new ServiceResponse<QuizQuestionResultDTO>();
            var faults = new List<string>();
            var created = new List<QuizQuestionResult>();
            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.QuizQuestionResultModel;
                    var isTransient = appletResultDTO.quizQuestionResultId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.quizQuestionResultId).Value;
                    appletResult = this.ConvertDto(appletResultDTO, appletResult);
                    sessionModel.RegisterSave(appletResult);
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new QuizQuestionResultDTO(x)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
            }

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
        public ServiceResponse<QuizQuestionResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<QuizQuestionResultDTO>();
            QuizQuestionResult quizResult;
            if ((quizResult = this.QuizQuestionResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new QuizQuestionResultDTO(quizResult);
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
            QuizQuestionResult quizResult;
            var model = this.QuizQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(quizResult, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="resultDTO">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResult"/>.
        /// </returns>
        private QuizQuestionResult ConvertDto(QuizQuestionResultDTO resultDTO, QuizQuestionResult instance)
        {
            instance = instance ?? new QuizQuestionResult();
            instance.Question = resultDTO.question;
            instance.IsCorrect = resultDTO.isCorrect;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value;
            instance.QuizResult = this.QuizResultModel.GetOneById(resultDTO.quizResultId).Value;
            instance.QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value;
            return instance;
        }

        #endregion
    }
}