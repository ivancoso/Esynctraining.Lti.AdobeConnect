using DotAmf.ServiceModel.Messaging;
using DotNetOpenAuth.Messaging;

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
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Resources;

    using FluentValidation.Results;
    using Newtonsoft.Json;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizResultService : BaseService, IQuizResultService
    {
        #region Properties

        private QuizResultModel _quizResultModel;
        private QuizResultModel QuizResultModel
        {
            get
            {
                return _quizResultModel ?? (_quizResultModel = IoC.Resolve<QuizResultModel>());
            }
        }

        private QuizModel _quizModel;
        private QuizModel QuizModel
        {
            get
            {
                return _quizModel ?? (_quizModel = IoC.Resolve<QuizModel>());
            }
        }

        private CompanyEventQuizMappingModel _eventQuizMappingModel;
        private CompanyEventQuizMappingModel EventQuizMappingModel
        {
            get
            {
                return _eventQuizMappingModel ?? (_eventQuizMappingModel = IoC.Resolve<CompanyEventQuizMappingModel>());
            }
        }

        private QuestionModel _questionModel;
        private QuestionModel QuestionModel
        {
            get
            {
                return _questionModel ?? (_questionModel = IoC.Resolve<QuestionModel>());
            }
        }

        private QuestionTypeModel _questionTypeModel;
        private QuestionTypeModel QuestionTypeModel
        {
            get { return _questionTypeModel ?? (_questionTypeModel = IoC.Resolve<QuestionTypeModel>()); }
        }


        private QuizQuestionResultModel _quizQuestionResultModel;
        private QuizQuestionResultModel QuizQuestionResultModel
        {
            get
            {
                return _quizQuestionResultModel ?? (_quizQuestionResultModel = IoC.Resolve<QuizQuestionResultModel>());
            }
        }

        private ConverterFactory _converterFactory;
        private ConverterFactory ConverterFactory
        {
            get { return _converterFactory ?? (_converterFactory = IoC.Resolve<ConverterFactory>()); }
        }

        private LmsUserParametersModel _lmsUserParametersModel;
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return _lmsUserParametersModel ?? (_lmsUserParametersModel = IoC.Resolve<LmsUserParametersModel>());
            }
        }

        private QuizQuestionResultAnswerModel _quizQuestionResultAnswerModel;
        private QuizQuestionResultAnswerModel QuizQuestionResultAnswerModel
        {
            get
            {
                return _quizQuestionResultAnswerModel ?? (_quizQuestionResultAnswerModel = IoC.Resolve<QuizQuestionResultAnswerModel>());
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task<OperationResultDto> SaveAll(QuizSummaryResultDTO quizResult)
        {
            if (quizResult == null)
                throw new ArgumentNullException(nameof(quizResult));
            if (quizResult.quizResults == null)
                quizResult.quizResults = new QuizResultDTO[0];

            IList<string> errorMessages = new List<string>();

            try
            {

                //TRICK to get eventQuizMappingId
                var eventQuizMappingId = GetEventQuizMappingId(quizResult);
                //

                Quiz quiz = QuizModel.GetOneById(quizResult.quizId).Value;
                ACSession acSession = ACSessionModel.GetOneById(quizResult.acSessionId).Value;
                if (acSession == null)
                {
                    throw new ArgumentException($"There are not session with acSessionId : {quizResult.acSessionId}");
                }
                int acSessionId = acSession.Id;

                CompanyEventQuizMapping companyEventQuizMapping = null;
                if (eventQuizMappingId.HasValue && eventQuizMappingId.Value != 0)
                    companyEventQuizMapping = EventQuizMappingModel.GetOneById(eventQuizMappingId.Value).Value;

                if (!IsValid(quizResult, out ValidationResult validationSummaryDtoResult))
                {
                    errorMessages = UpdateResultToShortString(validationSummaryDtoResult);
                    return OperationResultDto.Error(string.Join(";", errorMessages));
                }

                foreach (var appletResultDTO in quizResult.quizResults)
                {
                    appletResultDTO.acSessionId = quizResult.acSessionId;
                    if (IsValid(appletResultDTO, out ValidationResult validationResult))
                    {
                        var appletResult = ConvertDto(appletResultDTO, quiz, acSessionId, companyEventQuizMapping);
                        QuizResultModel.RegisterSave(appletResult);

                        if (appletResultDTO.results != null)
                        {
                            await SaveAllAsync(appletResult, appletResultDTO.results);
                        }
                    }
                    else
                    {
                        errorMessages.AddRange(UpdateResultToShortString(validationResult));
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Logger.Error("QuizResultService.SaveAll:", ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"QuizResultService.SaveAll json={JsonConvert.SerializeObject(quizResult)}", ex);

                throw;
            }

            return errorMessages.Any() 
                ? OperationResultDto.Error(string.Join(";", errorMessages)) 
                : OperationResultDto.Success();
        }

        public async Task<EventQuizResultDTO> GetByGuid(Guid guid)
        {
            QuizResult quizResult;
            if ((quizResult = this.QuizResultModel.GetOneByGuid(guid).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new EventQuizResultDTO(quizResult);
        }

        #endregion

        #region Methods

        private int? GetEventQuizMappingId(QuizSummaryResultDTO result)
        {
            int? eventQuizMappingId = null;

            if (!result.quizResults.Any())
                return eventQuizMappingId;

            if (result.eventQuizMappingId.HasValue && result.eventQuizMappingId.Value != 0)
                return result.eventQuizMappingId;

            var quizResults = this.QuizResultModel.GetQuizResultsByAcSessionId(result.acSessionId);
            var quizResult = quizResults.FirstOrDefault(q => q.EventQuizMapping != null);
            eventQuizMappingId = quizResult?.EventQuizMapping.Id ?? eventQuizMappingId;
            return eventQuizMappingId;
        }

        private static QuizResult ConvertDto(QuizResultDTO resultDTO, Quiz quiz, int acSessionId, CompanyEventQuizMapping companyEventQuizMapping)
        {
            var instance = new QuizResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp();
            instance.EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp();
            instance.Email = resultDTO.email?.Trim();
            instance.ACEmail = resultDTO.acEmail?.Trim();
            instance.IsArchive = resultDTO.isArchive;
            instance.DateCreated = DateTime.Now;

            instance.AppInFocusTime = resultDTO.appInFocusTime;
            instance.AppMaximizedTime = resultDTO.appMaximizedTime;
            instance.Guid = resultDTO.guid;
            instance.ParticipantName = resultDTO.participantName?.Trim();

            /**/
            instance.Quiz = quiz;
            instance.ACSessionId = acSessionId;
            instance.EventQuizMapping = companyEventQuizMapping;
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
                QuestionType = QuestionTypeModel.GetById(resultDTO.questionTypeId),
                QuizResult = quizResult,
                QuestionRef = QuestionModel.GetOneById(resultDTO.questionId).Value
            };

            return instance;
        }

        private async Task SaveAllAsync(QuizResult quizResult, QuizQuestionResultDTO[] results)
        {
            foreach (QuizQuestionResultDTO appletResultDTO in results)
            {
                if (IsValid(appletResultDTO, out var validationResult))
                {
                    var appletResult = ConvertDto(appletResultDTO, quizResult);
                    QuizQuestionResultModel.RegisterSave(appletResult);
                    if (appletResultDTO.answers != null && appletResultDTO.answers.Any())
                    {
                        CreateAnswers(appletResultDTO, appletResult);
                    }
                }
            }

            await ConvertAndSendQuizResultAsync(quizResult, results);
        }

        private void CreateAnswers(QuizQuestionResultDTO dto, QuizQuestionResult result)
        {
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
            }

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