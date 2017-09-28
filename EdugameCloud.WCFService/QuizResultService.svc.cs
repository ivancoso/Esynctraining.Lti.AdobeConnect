namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Threading.Tasks;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.Converters;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;
    using Newtonsoft.Json;

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

        public async Task<QuizResultSaveAllDTO> SaveAll(QuizSummaryResultDTO quizResult)
        {
            quizResult = quizResult ?? new QuizSummaryResultDTO { quizResults = new QuizResultDTO[] {} };

            try
            {
                var result = new QuizResultSaveAllDTO();

                //TRICK to get eventQuizMappingId
                var eventQuizMappingId = GetEventQuizMappingId(quizResult);
                //


                foreach (var appletResultDTO in quizResult.quizResults)
                {
                    ValidationResult validationResult;
                    if (this.IsValid(appletResultDTO, out validationResult))
                    {
                        var sessionModel = this.QuizResultModel;

                        var appletResult = this.ConvertDto(appletResultDTO, eventQuizMappingId, quizResult);
                        sessionModel.RegisterSave(appletResult);

                        await SaveAllAsync(appletResult, appletResultDTO.results);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"QuizResultService.SaveAll json={JsonConvert.SerializeObject(quizResult)}", ex);

                throw;
            }
        }

        #endregion

        #region Methods

        private int? GetEventQuizMappingId(QuizSummaryResultDTO result)
        {
            int? eventQuizMappingId = null;

            if (!result.quizResults.Any())
                return eventQuizMappingId;

            if (result.eventQuizMappingId.HasValue)
                return result.eventQuizMappingId;

            var quizResults = this.QuizResultModel.GetQuizResultsByAcSessionId(result.acSessionId);
            var quizResult = quizResults.FirstOrDefault(q => q.EventQuizMapping != null);
            eventQuizMappingId = quizResult?.EventQuizMapping.Id ?? eventQuizMappingId;
            return eventQuizMappingId;
        }

        private QuizResult ConvertDto(QuizResultDTO resultDTO, int? eventQuizMappingId, QuizSummaryResultDTO quizResult)
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
            instance.Quiz = this.QuizModel.GetOneById(quizResult.quizId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(quizResult.acSessionId).Value.With(x => x.Id);
            if (eventQuizMappingId.HasValue && eventQuizMappingId.Value != 0)
                instance.EventQuizMapping = EventQuizMappingModel.GetOneById(eventQuizMappingId.Value).Value;
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

        private async Task<QuizQuestionResultSaveAllDTO> SaveAllAsync(QuizResult quizResult, QuizQuestionResultDTO[] results)
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

            await this.ConvertAndSendQuizResultAsync(quizResult, results);

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

        private async Task ConvertAndSendQuizResultAsync(QuizResult quizResult, IEnumerable<QuizQuestionResultDTO> results)
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

            await converter.ConvertAndSendQuizResultToLmsAsync(results, quizResult, lmsUserParameters);
        }

        #endregion
    }
}