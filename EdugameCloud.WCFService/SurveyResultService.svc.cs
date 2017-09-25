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
    public class SurveyResultService : BaseService, ISurveyResultService
    {
        #region Properties

        private SurveyResultModel SurveyResultModel => IoC.Resolve<SurveyResultModel>();

        private SurveyModel SurveyModel => IoC.Resolve<SurveyModel>();

        private SurveyQuestionResultModel SurveyQuestionResultModel => IoC.Resolve<SurveyQuestionResultModel>();

        private QuestionTypeModel QuestionTypeModel => IoC.Resolve<QuestionTypeModel>();

        private QuestionModel QuestionModel => IoC.Resolve<QuestionModel>();

        private SurveyQuestionResultAnswerModel SurveyQuestionResultAnswerModel => IoC.Resolve<SurveyQuestionResultAnswerModel>();

        private DistractorModel DistractorModel => IoC.Resolve<DistractorModel>();

        private ConverterFactory ConverterFactory => IoC.Resolve<ConverterFactory>();

        private LmsUserParametersModel LmsUserParametersModel => IoC.Resolve<LmsUserParametersModel>();

        #endregion

        #region Public Methods and Operators

        public SurveyResultSaveAllDTO SaveAll(SurveyResultDTO[] results)
        {
            results = results ?? new SurveyResultDTO[] { };

            try
            {
                var result = new SurveyResultSaveAllDTO();
                var faults = new List<string>();
                var created = new List<SurveyResultSaveResultDTO>();
                foreach (var surveyResultDTO in results)
                {
                    ValidationResult validationResult;
                    if (this.IsValid(surveyResultDTO, out validationResult))
                    {
                        var surveyResultModel = this.SurveyResultModel;
                        var surveyResult = this.ConvertDto(surveyResultDTO);
                        surveyResultModel.RegisterSave(surveyResult);

                        var surveySaveResult = new SurveyResultSaveResultDTO(surveyResult);
                        created.Add(surveySaveResult);

                        var surveyQuestionResult = SaveAll(surveyResult, surveyResultDTO.results);
                        surveySaveResult.surveyQuestionResult = surveyQuestionResult;
                    }
                    else
                    {
                        faults.AddRange(this.UpdateResultToString(validationResult));
                    }
                }

                if (created.Any())
                {
                    //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<SurveyResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                    result.saved = created.ToArray();
                }

                if (faults.Any())
                {
                    result.faults = faults.ToArray();
                }

                return result;
            }
            catch(Exception ex)
            {
                Logger.Error($"SurveyResultService.SaveAll json={JsonConvert.SerializeObject(results)}", ex);

                throw;
            }
        }

        #endregion

        #region Methods

        private SurveyResult ConvertDto(SurveyResultDTO resultDTO)
        {
            var instance = new SurveyResult
            {
                Score = resultDTO.score,
                StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp(),
                EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp(),
                Email = resultDTO.email.With(x => x.Trim()),
                IsArchive = resultDTO.isArchive,
                DateCreated = DateTime.Now,
                ParticipantName = resultDTO.participantName.With(x => x.Trim()),
                Survey = this.SurveyModel.GetOneById(resultDTO.surveyId).Value,
                ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id),
                LmsUserParametersId =
                    resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null,
                ACEmail = resultDTO.acEmail.With(x => x.Trim())
            };

            return instance;
        }

        private SurveyQuestionResult ConvertDto(SurveyQuestionResultDTO resultDTO, SurveyQuestionResult instance, SurveyResult surveyResult)
        {
            instance = instance ?? new SurveyQuestionResult();
            instance.Question = resultDTO.question;
            instance.IsCorrect = resultDTO.isCorrect;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value;
            instance.SurveyResult = surveyResult;
            instance.QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value;
            return instance;
        }

        private SurveyQuestionResultAnswer ConvertDto(SurveyQuestionResultAnswerDTO resultDTO, SurveyQuestionResultAnswer instance, SurveyQuestionResult result)
        {
            instance = instance ?? new SurveyQuestionResultAnswer();
            instance.Value = resultDTO.value;
            instance.SurveyDistractorAnswer = resultDTO.surveyDistractorAnswerId.HasValue ? this.DistractorModel.GetOneById(resultDTO.surveyDistractorAnswerId.Value).Value : null;
            instance.SurveyDistractor = resultDTO.surveyDistractorId.HasValue ? this.DistractorModel.GetOneById(resultDTO.surveyDistractorId.Value).Value : null;
            instance.SurveyQuestionResult = result;
            return instance;
        }

        private SurveyQuestionResultSaveAllDTO SaveAll(SurveyResult instance, SurveyQuestionResultDTO[] results)
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
                    surveyQuestionResult = this.ConvertDto(surveyQuestionResultDTO, surveyQuestionResult, instance);
                    sessionModel.RegisterSave(surveyQuestionResult, true);
                    if (isTransient && surveyQuestionResultDTO.answers != null && surveyQuestionResultDTO.answers.Any())
                    {
                        var answers = this.CreateAnswers(surveyQuestionResultDTO, surveyQuestionResult, this.SurveyQuestionResultAnswerModel);
                        foreach (var answ in answers)
                            surveyQuestionResult.Answers.Add(answ);
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

            this.ConvertAndSendSurveyResult(instance, results);

            return result;
        }

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
                        Logger.ErrorFormat("Saving answers:" + ex);
                    }
                    finally
                    {
                        created.Add(answer);
                    }
                }
            }

            return created;
        }

        // TODO: review
        private void ConvertAndSendSurveyResult(SurveyResult surveyResult, IEnumerable<SurveyQuestionResultDTO> results)
        {
            if (surveyResult == null)
                return;

            var lmsUserParameters = surveyResult.LmsUserParametersId.HasValue
                ? this.LmsUserParametersModel.GetOneById(surveyResult.LmsUserParametersId.Value).Value
                : null;

            if (lmsUserParameters == null)
            {
                return;
            }

            var lmsSurveyId = surveyResult.Survey.LmsSurveyId;
            if (lmsSurveyId == null)
                return;

            var converter =
                this.ConverterFactory.GetResultConverter((LmsProviderEnum) lmsUserParameters.CompanyLms.LmsProviderId);

            converter.ConvertAndSendSurveyResultToLms(results, surveyResult, lmsUserParameters);
        }

        #endregion

    }

}