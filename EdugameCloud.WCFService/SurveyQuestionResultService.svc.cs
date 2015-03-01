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
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.Converters;

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
        ///     The <see cref="SurveyQuestionResultDTO" />.
        /// </returns>
        public SurveyQuestionResultDTO[] GetAll()
        {
            return this.SurveyQuestionResultModel.GetAll().Select(x => new SurveyQuestionResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyQuestionResultDTO"/>.
        /// </returns>
        public SurveyQuestionResultDTO Save(SurveyQuestionResultDTO resultDto)
        {
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
                    var answers = this.CreateAnswers(resultDto, surveyQuestionResult, this.SurveyQuestionResultAnswerModel);
                    surveyQuestionResult.Answers.AddAll(answers);
                }

                return new SurveyQuestionResultDTO(surveyQuestionResult, surveyQuestionResult.Answers);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SurveyQuestionResult.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyQuestionResultSaveAllDTO"/>.
        /// </returns>
        public SurveyQuestionResultSaveAllDTO SaveAll(SurveyQuestionResultDTO[] results)
        {
            results = results ?? new SurveyQuestionResultDTO[] { };
            var result = new SurveyQuestionResultSaveAllDTO();
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
                        var answers = this.CreateAnswers(surveyQuestionResultDTO, surveyQuestionResult, this.SurveyQuestionResultAnswerModel);
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
                result.saved = created.Select(x => new SurveyQuestionResultDTO(x, x.Answers)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }
            
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
        /// The <see cref="SurveyQuestionResultDTO"/>.
        /// </returns>
        public SurveyQuestionResultDTO GetById(int id)
        {
            SurveyQuestionResult quizResult;
            if ((quizResult = this.SurveyQuestionResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SurveyQuestionResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyQuestionResultDTO(quizResult, quizResult.Answers);
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
            SurveyQuestionResult quizResult;
            var model = this.SurveyQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuestionType.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            return id;
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
        /// <returns>
        /// The <see cref="List{SurveyQuestionResultAnswer}"/>.
        /// </returns>
        private List<SurveyQuestionResultAnswer> CreateAnswers(SurveyQuestionResultDTO dto, SurveyQuestionResult result, SurveyQuestionResultAnswerModel answerModel)
        {
            var created = new List<SurveyQuestionResultAnswer>();
            foreach (var answerDTO in dto.answers)
            {
                answerDTO.surveyQuestionResultId = result.Id;
                ValidationResult distractorValidationResult;
                if (this.IsValid(answerDTO, out distractorValidationResult))
                {
                    SurveyQuestionResultAnswer answer = null;
                    try
                    {
                        var isAnswerTransient = answerDTO.surveyQuestionResultAnswerId == 0;
                        answer = isAnswerTransient
                                     ? null
                                     : answerModel.GetOneById(answerDTO.surveyQuestionResultAnswerId).Value;
                        answer = this.ConvertDto(answerDTO, answer, result);
                        answerModel.RegisterSave(answer, true);
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("Saving answers:" + ex);
                    }
                    finally
                    {
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