﻿// ReSharper disable once CheckNamespace
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
    //using EdugameCloud.Core.RTMP;
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

        public async Task SaveAll(SurveySummaryResultDTO sResult)
        {
            sResult = sResult ?? new SurveySummaryResultDTO { surveyResults = new SurveyResultDTO[] {}};

            try
            {
                foreach (var surveyResultDTO in sResult.surveyResults)
                {
                    ValidationResult validationResult;
                    if (this.IsValid(surveyResultDTO, out validationResult))
                    {
                        var surveyResultModel = this.SurveyResultModel;
                        var surveyResult = this.ConvertDto(surveyResultDTO, sResult);
                        surveyResultModel.RegisterSave(surveyResult);
                        await SaveAllAsync(surveyResult, surveyResultDTO.results);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error($"SurveyResultService.SaveAll json={JsonConvert.SerializeObject(sResult)}", ex);

                throw;
            }
        }

        #endregion

        #region Methods

        private SurveyResult ConvertDto(SurveyResultDTO resultDTO, SurveySummaryResultDTO sResult)
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
                Survey = this.SurveyModel.GetOneById(sResult.surveyId).Value,
                ACSessionId = this.ACSessionModel.GetOneById(sResult.acSessionId).Value.With(x => x.Id),
                LmsUserParametersId =
                    resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null,
                ACEmail = resultDTO.acEmail.With(x => x.Trim())
            };

            return instance;
        }

        private SurveyQuestionResult ConvertDto(SurveyQuestionResultDTO resultDTO, SurveyResult surveyResult)
        {
            var instance = new SurveyQuestionResult
            {
                Question = resultDTO.question,
                IsCorrect = resultDTO.isCorrect,
                QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value,
                SurveyResult = surveyResult,
                QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value
            };

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

        private async Task SaveAllAsync(SurveyResult instance, SurveyQuestionResultDTO[] results)
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
                    var surveyQuestionResult = this.ConvertDto(surveyQuestionResultDTO, instance);
                    sessionModel.RegisterSave(surveyQuestionResult, true);
                    if (surveyQuestionResultDTO.answers != null && surveyQuestionResultDTO.answers.Any())
                    {
                        this.CreateAnswers(surveyQuestionResultDTO, surveyQuestionResult, this.SurveyQuestionResultAnswerModel);
                    }

                }
            }

            await this.ConvertAndSendSurveyResultAsync(instance, results);

        }

        private void CreateAnswers(SurveyQuestionResultDTO dto, SurveyQuestionResult result, SurveyQuestionResultAnswerModel answerModel)
        {
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
                }
            }
        }

        // TODO: review
        private async Task ConvertAndSendSurveyResultAsync(SurveyResult surveyResult, IEnumerable<SurveyQuestionResultDTO> results)
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

            await converter.ConvertAndSendSurveyResultToLmsAsync(results, surveyResult, lmsUserParameters);
        }

        #endregion

    }

}