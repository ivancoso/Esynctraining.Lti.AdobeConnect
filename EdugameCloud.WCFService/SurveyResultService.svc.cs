// ReSharper disable once CheckNamespace
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

        private SurveyResultModel _surveyResultModel;
        private SurveyResultModel SurveyResultModel
        {
            get
            {
                return _surveyResultModel ?? (_surveyResultModel = IoC.Resolve<SurveyResultModel>());
            }
        }

        private SurveyModel _surveyModel;
        private SurveyModel SurveyModel
        {
            get
            {
                return _surveyModel ?? (_surveyModel = IoC.Resolve<SurveyModel>());
            }
        }

        private SurveyQuestionResultModel _surveyQuestionResultModel;
        private SurveyQuestionResultModel SurveyQuestionResultModel
        {
            get
            {
                return _surveyQuestionResultModel ?? (_surveyQuestionResultModel = IoC.Resolve<SurveyQuestionResultModel>());
            }
        }

        private QuestionTypeModel _questionTypeModel;
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return _questionTypeModel ?? (_questionTypeModel = IoC.Resolve<QuestionTypeModel>());
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

        private SurveyQuestionResultAnswerModel _surveyQuestionResultAnswerModel;
        private SurveyQuestionResultAnswerModel SurveyQuestionResultAnswerModel
        {
            get
            {
                return _surveyQuestionResultAnswerModel ?? (_surveyQuestionResultAnswerModel = IoC.Resolve<SurveyQuestionResultAnswerModel>());
            }
        }

        private DistractorModel _distractorModel;
        private DistractorModel DistractorModel
        {
            get
            {
                return _distractorModel ?? (_distractorModel = IoC.Resolve<DistractorModel>());
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
            get { return _lmsUserParametersModel ?? (_lmsUserParametersModel = IoC.Resolve<LmsUserParametersModel>()); }
        }

        #endregion

        #region Public Methods and Operators

        public async Task SaveAll(SurveySummaryResultDTO sResult)
        {
            if (sResult == null)
                throw new ArgumentNullException(nameof(sResult));
            if (sResult.surveyResults == null)
                sResult.surveyResults = new SurveyResultDTO[0];
            
            try
            {

                if (!this.IsValid(sResult, out ValidationResult valResult))
                    return;

                Survey survey = this.SurveyModel.GetOneById(sResult.surveyId).Value;
                int acSessionId = this.ACSessionModel.GetOneById(sResult.acSessionId).Value.With(x => x.Id);
                foreach (var surveyResultDTO in sResult.surveyResults)
                {
                    surveyResultDTO.acSessionId = sResult.acSessionId;
                    ValidationResult validationResult;
                    if (this.IsValid(surveyResultDTO, out validationResult))
                    {
                        var surveyResultModel = this.SurveyResultModel;
                        var surveyResult = this.ConvertDto(surveyResultDTO, survey, acSessionId);
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

        private SurveyResult ConvertDto(SurveyResultDTO resultDTO, Survey survey, int acSessionId)
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
                Survey = survey,
                ACSessionId = acSessionId,
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