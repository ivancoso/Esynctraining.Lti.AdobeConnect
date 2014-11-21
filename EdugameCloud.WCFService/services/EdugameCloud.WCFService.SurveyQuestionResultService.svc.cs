// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Converters;
    using EdugameCloud.WCFService.Base;
    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;
    using Resources;

    /// <summary>
    ///     The SurveyQuestionResult service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SurveyQuestionResultService : BaseService, ISurveyQuestionResultService
    {
        #region Properties

        /// <summary>
        /// Gets the survey result model.
        /// </summary>
        private SurveyResultModel SurveyResultModel
        {
            get
            {
                return IoC.Resolve<SurveyResultModel>();
            }
        }

        /// <summary>
        /// Gets the distractor result model.
        /// </summary>
        private DistractorModel DistractorModel
        {
            get
            {
                return IoC.Resolve<DistractorModel>();
            }
        }

        /// <summary>
        /// Gets the survey question result model.
        /// </summary>
        private SurveyQuestionResultModel SurveyQuestionResultModel
        {
            get
            {
                return IoC.Resolve<SurveyQuestionResultModel>();
            }
        }

        /// <summary>
        /// Gets the survey question result answer model.
        /// </summary>
        private SurveyQuestionResultAnswerModel SurveyQuestionResultAnswerModel
        {
            get
            {
                return IoC.Resolve<SurveyQuestionResultAnswerModel>();
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

        /// <summary>
        /// Gets the quiz result converter.
        /// </summary>
        private QuizResultConverter QuizResultConverter
        {
            get
            {
                return IoC.Resolve<QuizResultConverter>();
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
        public ServiceResponse<SurveyQuestionResultDTO> GetAll()
        {
            return new ServiceResponse<SurveyQuestionResultDTO> { objects = this.SurveyQuestionResultModel.GetAll().Select(x => new SurveyQuestionResultDTO(x)).ToList() };
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
        public ServiceResponse<SurveyQuestionResultDTO> Save(SurveyQuestionResultDTO resultDto)
        {
            var result = new ServiceResponse<SurveyQuestionResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var surveyQuestionResultModel = this.SurveyQuestionResultModel;
                var isTransient = resultDto.surveyQuestionResultId == 0;
                var surveyQuestionResult = isTransient ? null : surveyQuestionResultModel.GetOneById(resultDto.surveyQuestionResultId).Value;
                surveyQuestionResult = this.ConvertDto(resultDto, surveyQuestionResult);
                surveyQuestionResultModel.RegisterSave(surveyQuestionResult);
                
                if (isTransient && resultDto.answers != null && resultDto.answers.Any())
                {
                    var answers = this.CreateAnswers(resultDto, surveyQuestionResult, this.SurveyQuestionResultAnswerModel, result);
                    surveyQuestionResult.Answers.AddAll(answers);
                }

                result.@object = new SurveyQuestionResultDTO(surveyQuestionResult, surveyQuestionResult.Answers);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
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
        public ServiceResponse<SurveyQuestionResultDTO> SaveAll(List<SurveyQuestionResultDTO> results)
        {
            var result = new ServiceResponse<SurveyQuestionResultDTO>();
            var faults = new List<string>();
            var created = new List<SurveyQuestionResult>();
            foreach (var surveyQuestionResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(surveyQuestionResultDTO, out validationResult))
                {
                    var sessionModel = this.SurveyQuestionResultModel;
                    var isTransient = surveyQuestionResultDTO.surveyQuestionResultId == 0;
                    var surveyQuestionResult = isTransient ? null : sessionModel.GetOneById(surveyQuestionResultDTO.surveyQuestionResultId).Value;
                    surveyQuestionResult = this.ConvertDto(surveyQuestionResultDTO, surveyQuestionResult);
                    sessionModel.RegisterSave(surveyQuestionResult, true);
                    if (isTransient && surveyQuestionResultDTO.answers != null && surveyQuestionResultDTO.answers.Any())
                    {
                        var answers = this.CreateAnswers(surveyQuestionResultDTO, surveyQuestionResult, this.SurveyQuestionResultAnswerModel, result);
                        surveyQuestionResult.Answers.AddAll(answers);
                    }

                    created.Add(surveyQuestionResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new SurveyQuestionResultDTO(x, x.Answers)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
            }

            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            
            QuizResultConverter.ConvertAndSendSurveyResult(results);
            
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
        public ServiceResponse<SurveyQuestionResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<SurveyQuestionResultDTO>();
            SurveyQuestionResult quizResult;
            if ((quizResult = this.SurveyQuestionResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SurveyQuestionResultDTO(quizResult, quizResult.Answers);
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
            SurveyQuestionResult quizResult;
            var model = this.SurveyQuestionResultModel;
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
        /// The create distractors.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="answerModel">
        /// The distractor model.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <returns>
        /// The <see cref="List{SurveyQuestionResultAnswer}"/>.
        /// </returns>
        private List<SurveyQuestionResultAnswer> CreateAnswers(SurveyQuestionResultDTO dto, SurveyQuestionResult result, SurveyQuestionResultAnswerModel answerModel, ServiceResponse<SurveyQuestionResultDTO> response)
        {
            var created = new List<SurveyQuestionResultAnswer>();
            foreach (var answerDTO in dto.answers)
            {
                answerDTO.surveyQuestionResultId = result.Id;
                ValidationResult distractorValidationResult;
                if (this.IsValid(answerDTO, out distractorValidationResult))
                {
                    SurveyQuestionResultAnswer answer = null;
                    bool savedSuccessfully = false;
                    try
                    {
                        var isAnswerTransient = answerDTO.surveyQuestionResultAnswerId == 0;
                        answer = isAnswerTransient
                                     ? null
                                     : answerModel.GetOneById(answerDTO.surveyQuestionResultAnswerId).Value;
                        answer = this.ConvertDto(answerDTO, answer, result);
                        answerModel.RegisterSave(answer, true);
                        savedSuccessfully = true;
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("Saving answers:" + ex);
                    }
                    finally
                    {
                        if (answer != null && savedSuccessfully && response.@object != null)
                        {
                            response.@object.answers = response.@object.answers ?? new List<SurveyQuestionResultAnswerDTO>();
                            response.@object.answers.Add(new SurveyQuestionResultAnswerDTO(answer));
                        }

                        created.Add(answer);
                    }
                }
            }

            return created;
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="resultDTO">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResult"/>.
        /// </returns>
        private SurveyQuestionResultAnswer ConvertDto(SurveyQuestionResultAnswerDTO resultDTO, SurveyQuestionResultAnswer instance, SurveyQuestionResult result)
        {
            instance = instance ?? new SurveyQuestionResultAnswer();
            instance.Value = resultDTO.value;
            instance.SurveyDistractorAnswer = resultDTO.surveyDistractorAnswerId.HasValue ? this.DistractorModel.GetOneById(resultDTO.surveyDistractorAnswerId.Value).Value : null;
            instance.SurveyDistractor = resultDTO.surveyDistractorId.HasValue ? this.DistractorModel.GetOneById(resultDTO.surveyDistractorId.Value).Value : null;
            instance.SurveyQuestionResult = result;
            return instance;
        }

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
        private SurveyQuestionResult ConvertDto(SurveyQuestionResultDTO resultDTO, SurveyQuestionResult instance)
        {
            instance = instance ?? new SurveyQuestionResult();
            instance.Question = resultDTO.question;
            instance.IsCorrect = resultDTO.isCorrect;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value;
            instance.SurveyResult = this.SurveyResultModel.GetOneById(resultDTO.surveyResultId).Value;
            instance.QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value;
            return instance;
        }

        #endregion
    }
}