// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    //using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.Converters;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;
    using Newtonsoft.Json;
    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizResultService : BaseService, IQuizResultService
    {
        #region Properties

        private QuizResultModel QuizResultModel => IoC.Resolve<QuizResultModel>();

        private QuizModel QuizModel => IoC.Resolve<QuizModel>();
        private CompanyEventQuizMappingModel EventQuizMappingModel => IoC.Resolve<CompanyEventQuizMappingModel>();

        private QuestionModel QuestionModel => IoC.Resolve<QuestionModel>();

        private QuestionTypeModel QuestionTypeModel => IoC.Resolve<QuestionTypeModel>();

        private QuizQuestionResultModel QuizQuestionResultModel => IoC.Resolve<QuizQuestionResultModel>();

        private ConverterFactory ConverterFactory => IoC.Resolve<ConverterFactory>();

        private LmsUserParametersModel LmsUserParametersModel => IoC.Resolve<LmsUserParametersModel>();

        private QuizQuestionResultAnswerModel QuizQuestionResultAnswerModel => IoC.Resolve<QuizQuestionResultAnswerModel>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All list.
        /// </summary>
        /// <returns>
        ///     The <see cref="QuizResultDTO" />.
        /// </returns>
        public QuizResultDTO[] GetAll()
        {
            return this.QuizResultModel.GetAll().Select(x => new QuizResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultSaveAllDTO"/>.
        /// </returns>
        public QuizResultSaveAllDTO SaveAll(QuizResultDTO[] results)
        {
            results = results ?? new QuizResultDTO[] { };

            try
            {
                var result = new QuizResultSaveAllDTO();
                var faults = new List<string>();
                var created = new List<QuizResultSaveResultDTO>();
                foreach (var appletResultDTO in results)
                {
                    ValidationResult validationResult;
                    if (this.IsValid(appletResultDTO, out validationResult))
                    {
                        var sessionModel = this.QuizResultModel;
                        var isTransient = appletResultDTO.quizResultId == 0;
                        var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.quizResultId).Value;
                        appletResult = this.ConvertDto(appletResultDTO, appletResult);
                        sessionModel.RegisterSave(appletResult);

                        var quizSaveResult = new QuizResultSaveResultDTO(appletResult);
                        created.Add(quizSaveResult);

                        var quizQuestionResult = SaveAll(appletResult, appletResultDTO.results);
                        quizSaveResult.quizQuestionResult = quizQuestionResult;
                    }
                    else
                    {
                        faults.AddRange(this.UpdateResultToString(validationResult));
                    }
                }

                if (created.Any())
                {
                    //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                    result.saved = created.ToArray();
                }

                if (faults.Any())
                {
                    result.faults = faults.ToArray();
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"QuizResultService.SaveAll json={JsonConvert.SerializeObject(results)}", ex);

                throw;
            }
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDTO"/>.
        /// </returns>
        public QuizResultDTO GetById(int id)
        {
            QuizResult quizResult;
            if ((quizResult = this.QuizResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizResultDTO(quizResult);
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
            QuizResult quizResult;
            var model = this.QuizResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Delete, quizResult.With(x => x.Quiz).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id), quizResult.Id);
            return id;
        }

        public QuizResultDTO GetByGuid(Guid guid)
        {
            QuizResult quizResult;
            if ((quizResult = this.QuizResultModel.GetOneByGuid(guid).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizResultDTO(quizResult);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="resultDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResult"/>.
        /// </returns>
        private QuizResult ConvertDto(QuizResultDTO resultDTO, QuizResult instance)
        {
            instance = instance ?? new QuizResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp();
            instance.EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp();
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.ACEmail = resultDTO.acEmail.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.AppInFocusTime = resultDTO.appInFocusTime;
            instance.AppMaximizedTime = resultDTO.appMaximizedTime;
            instance.Guid = resultDTO.guid;
            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.Quiz = this.QuizModel.GetOneById(resultDTO.quizId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.LmsId = resultDTO.lmsId;
            instance.LmsUserParametersId = resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null;
            instance.isCompleted = resultDTO.isCompleted;
            if (resultDTO.eventQuizMappingId.HasValue && resultDTO.eventQuizMappingId.Value != 0)
                instance.EventQuizMapping = EventQuizMappingModel.GetOneById(resultDTO.eventQuizMappingId.Value).Value;
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

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResultDTO"/>.
        /// </returns>
        private QuizQuestionResultSaveAllDTO SaveAll(QuizResult quizResult, QuizQuestionResultDTO[] results)
        {
            results = results ?? new QuizQuestionResultDTO[] { };

            foreach (var item in results)
            {
                item.quizResultId = quizResult.Id;
            }

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
                        case (int)QuestionTypeEnum.SingleMultipleChoiceText:
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

        #endregion
    }
}