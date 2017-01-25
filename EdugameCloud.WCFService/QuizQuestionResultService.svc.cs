// ReSharper disable once CheckNamespace

using System;

namespace EdugameCloud.WCFService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.Converters;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;
    using FluentValidation.Results;
    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizQuestionResultService : BaseService, IQuizQuestionResultService
    {
        #region Properties

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
        /// Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        private ConverterFactory ConverterFactory
        {
            get
            {
                return IoC.Resolve<ConverterFactory>();
            }
        }

        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        private QuizQuestionResultAnswerModel QuizQuestionResultAnswerModel
        {
            get
            {
                return IoC.Resolve<QuizQuestionResultAnswerModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

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
            QuizQuestionResult quizResult;
            QuizQuestionResultModel model = this.QuizQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizQuestionResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            return id;
        }

        /// <summary>
        /// Gets all quiz questions results.
        /// </summary>
        /// <returns>
        ///     The <see cref="QuizQuestionResultDTO" />.
        /// </returns>
        public QuizQuestionResultDTO[] GetAll()
        {
            return this.QuizQuestionResultModel.GetAll().Select(x => new QuizQuestionResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResultDTO"/>.
        /// </returns>
        public QuizQuestionResultDTO GetById(int id)
        {
            QuizQuestionResult quizResult;
            if ((quizResult = this.QuizQuestionResultModel.GetOneById(id).Value) == null)
            {
                var error = 
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizQuestionResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizQuestionResultDTO(quizResult);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResultDTO"/>.
        /// </returns>
        public QuizQuestionResultDTO Save(QuizQuestionResultDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                QuizQuestionResultModel quizQuestionResultModel = this.QuizQuestionResultModel;
                bool isTransient = resultDto.quizQuestionResultId == 0;
                QuizQuestionResult quizQuestionResult = isTransient
                                                            ? null
                                                            : quizQuestionResultModel.GetOneById(
                                                                resultDto.quizQuestionResultId).Value;
                quizQuestionResult = this.ConvertDto(resultDto, quizQuestionResult);
                quizQuestionResultModel.RegisterSave(quizQuestionResult);
                if (isTransient && resultDto.answers != null && resultDto.answers.Any())
                {
                    var answers = this.CreateAnswers(resultDto, quizQuestionResult);
                    foreach (var answ in answers)
                        quizQuestionResult.Answers.Add(answ);
                }
                return new QuizQuestionResultDTO(quizQuestionResult);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("QuizQuestion.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResultDTO"/>.
        /// </returns>
        public QuizQuestionResultSaveAllDTO SaveAll(QuizQuestionResultDTO[] results)
        {
            results = results ?? new QuizQuestionResultDTO[] { };
            var result = new QuizQuestionResultSaveAllDTO();
            var faults = new List<string>();
            var created = new List<QuizQuestionResult>();
            foreach (QuizQuestionResultDTO appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    QuizQuestionResultModel sessionModel = this.QuizQuestionResultModel;
                    bool isTransient = appletResultDTO.quizQuestionResultId == 0;
                    QuizQuestionResult appletResult = isTransient
                                                          ? null
                                                          : sessionModel.GetOneById(
                                                              appletResultDTO.quizQuestionResultId).Value;
                    appletResult = this.ConvertDto(appletResultDTO, appletResult);
                    sessionModel.RegisterSave(appletResult);
                    if (isTransient && appletResultDTO.answers != null && appletResultDTO.answers.Any())
                    {
                        var answers = this.CreateAnswers(appletResultDTO, appletResult);
                        foreach (var answ in answers)
                            appletResult.Answers.Add(answ);
                    }
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                result.saved = created.Select(x => new QuizQuestionResultDTO(x)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

            this.ConvertAndSendQuizResult(results);
            
            return result;
        }

        #endregion

        #region Methods

        private void ConvertAndSendQuizResult(IEnumerable<QuizQuestionResultDTO> results)
        {
            foreach (var userAnswer in results.GroupBy(r => r.quizResultId))
            {
                var quizResult = this.QuizResultModel.GetOneById(userAnswer.Key).Value;
                if (quizResult == null)
                {
                    continue;
                }

                var lmsUserParameters = quizResult.LmsUserParametersId.HasValue ? this.LmsUserParametersModel.GetOneById(quizResult.LmsUserParametersId.Value).Value : null;
                if (lmsUserParameters == null || lmsUserParameters.LmsUser == null)
                {
                    return;
                }

                var lmsQuizId = quizResult.Quiz.LmsQuizId;
                if (lmsQuizId == null)
                {
                    continue;
                }

                var converter = this.ConverterFactory.GetResultConverter((LmsProviderEnum)lmsUserParameters.CompanyLms.LmsProviderId);
                
                converter.ConvertAndSendQuizResultToLms(userAnswer, quizResult, lmsUserParameters);
            }
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

        private List<QuizQuestionResultAnswer> CreateAnswers(QuizQuestionResultDTO dto, QuizQuestionResult result)
        {
            var created = new List<QuizQuestionResultAnswer>();
            foreach (var answerString in dto.answers)
            {
                var answer = new QuizQuestionResultAnswer();
                try
                {
                    answer.QuizQuestionResult = result;
                    answer.Value = answerString;

                    switch (result.QuestionType.Id)
                    {
                        case (int) QuestionTypeEnum.SingleMultipleChoiceText:
                            int distractorId;
                            if (int.TryParse(answerString, out distractorId))
                            {
                                var distractor = result.QuestionRef.Distractors.FirstOrDefault(x => x.Id == distractorId);
                                if (distractor != null)
                                {
                                    answer.QuizDistractorAnswer = distractor;
                                    answer.Value = distractor.DistractorName;
                                }
                                else
                                {
                                    answer.Value = answerString;
                                }
                            }
                            else
                            {
                                answer.Value = answerString;
                            }

                            break;
                        default:
                            answer.Value = answerString;
                            break;
                    }

                    QuizQuestionResultAnswerModel.RegisterSave(answer);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Saving answers:" + ex);
                }
                finally
                {
                    created.Add(answer);
                }
            }

            return created;
        }

        #endregion
    }
}