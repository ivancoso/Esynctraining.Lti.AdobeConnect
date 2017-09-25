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

                        var appletResult = this.ConvertDto(appletResultDTO);
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

        #endregion

        #region Methods

        private QuizResult ConvertDto(QuizResultDTO resultDTO)
        {
            var instance = new QuizResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp();
            instance.EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp();
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.ACEmail = resultDTO.acEmail.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            instance.DateCreated = DateTime.Now;

            instance.AppInFocusTime = resultDTO.appInFocusTime;
            instance.AppMaximizedTime = resultDTO.appMaximizedTime;
            instance.Guid = resultDTO.guid;
            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());

            /**/
            instance.Quiz = this.QuizModel.GetOneById(resultDTO.quizId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            if (resultDTO.eventQuizMappingId.HasValue && resultDTO.eventQuizMappingId.Value != 0)
                instance.EventQuizMapping = EventQuizMappingModel.GetOneById(resultDTO.eventQuizMappingId.Value).Value;
            /**/

            instance.LmsId = resultDTO.lmsId;
            instance.LmsUserParametersId = resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null;
            instance.isCompleted = resultDTO.isCompleted;            
            return instance;
        }

        private QuizQuestionResult ConvertDto(QuizQuestionResultDTO resultDTO, QuizResult quizResult)
        {
            var instance = new QuizQuestionResult
            {
                Question = resultDTO.question,
                IsCorrect = resultDTO.isCorrect,
                QuestionType = QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value,
                QuizResult = quizResult,
                QuestionRef = QuestionModel.GetOneById(resultDTO.questionId).Value
            };

            return instance;
        }

        private QuizQuestionResultSaveAllDTO SaveAll(QuizResult quizResult, QuizQuestionResultDTO[] results)
        {
            results = results ?? new QuizQuestionResultDTO[] { };

            var result = new QuizQuestionResultSaveAllDTO();
            var faults = new List<string>();
            var created = new List<QuizQuestionResult>();
            foreach (QuizQuestionResultDTO appletResultDTO in results)
            {
                if (this.IsValid(appletResultDTO, out var validationResult))
                {
                    QuizQuestionResultModel sessionModel = this.QuizQuestionResultModel;
                    var appletResult = ConvertDto(appletResultDTO, quizResult);
                    sessionModel.RegisterSave(appletResult);
                    if (appletResultDTO.answers != null && appletResultDTO.answers.Any())
                    {
                        var answers = this.CreateAnswers(appletResultDTO, appletResult);
                        foreach (var answ in answers)
                            appletResult.Answers.Add(answ);
                    }
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(UpdateResultToString(validationResult));
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

            this.ConvertAndSendQuizResult(quizResult, results);

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
                            if (int.TryParse(answerString, out var distractorId))
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

        private void ConvertAndSendQuizResult(QuizResult quizResult, IEnumerable<QuizQuestionResultDTO> results)
        {
            if (quizResult == null)
                return;

            var lmsUserParameters = quizResult.LmsUserParametersId.HasValue
                ? this.LmsUserParametersModel.GetOneById(quizResult.LmsUserParametersId.Value).Value
                : null;

            if (lmsUserParameters == null || lmsUserParameters.LmsUser == null)
            {
                return;
            }

            var lmsQuizId = quizResult.Quiz.LmsQuizId;
            if (lmsQuizId == null)
                return;
            var converter =
                this.ConverterFactory.GetResultConverter((LmsProviderEnum) lmsUserParameters.CompanyLms.LmsProviderId);

            converter.ConvertAndSendQuizResultToLms(results, quizResult, lmsUserParameters);
        }

        #endregion
    }
}