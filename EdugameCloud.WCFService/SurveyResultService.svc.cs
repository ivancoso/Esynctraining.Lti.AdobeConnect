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

        /// <summary>
        ///     All results.
        /// </summary>
        /// <returns>
        ///     The <see cref="SurveyResultDTO" />.
        /// </returns>
        public SurveyResultDTO[] GetAll()
        {
            return this.SurveyResultModel.GetAll().Select(x => new SurveyResultDTO(x)).Take(10).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyResultSaveAllDTO"/>.
        /// </returns>
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
                        var isTransient = surveyResultDTO.surveyResultId == 0;
                        var surveyResult = isTransient ? null : surveyResultModel.GetOneById(surveyResultDTO.surveyResultId).Value;
                        surveyResult = this.ConvertDto(surveyResultDTO, surveyResult);
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

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyResultDTO"/>.
        /// </returns>
        public SurveyResultDTO GetById(int id)
        {
            SurveyResult surveyResult;
            if ((surveyResult = this.SurveyResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SurveyResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyResultDTO(surveyResult);
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
            SurveyResult surveyResult;
            var model = this.SurveyResultModel;
            if ((surveyResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("SurveyResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(surveyResult, true);
            //IoC.Resolve<RealTimeNotificationModel>()
            //    .NotifyClientsAboutChangesInTable<SurveyResult>(
            //        NotificationType.Delete,
            //        surveyResult.With(x => x.Survey)
            //            .With(x => x.SubModuleItem)
            //            .With(x => x.CreatedBy)
            //            .With(x => x.Company.Id),
            //        surveyResult.Id);
            return id;
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
        private SurveyResult ConvertDto(SurveyResultDTO resultDTO, SurveyResult instance)
        {
            instance = instance ?? new SurveyResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp();
            instance.EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp();
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.Survey = this.SurveyModel.GetOneById(resultDTO.surveyId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.LmsUserParametersId = resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null;
            instance.ACEmail = resultDTO.acEmail.With(x => x.Trim());
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
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyQuestionResultSaveAllDTO"/>.
        /// </returns>
        private SurveyQuestionResultSaveAllDTO SaveAll(SurveyResult instance, SurveyQuestionResultDTO[] results)
        {
            results = results ?? new SurveyQuestionResultDTO[] { };

            foreach (var item in results)
            {
                item.surveyResultId = instance.Id;
            }

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

            this.ConvertAndSendSurveyResult(results);

            return result;
        }

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
        private void ConvertAndSendSurveyResult(IEnumerable<SurveyQuestionResultDTO> results)
        {
            foreach (var userAnswer in results.GroupBy(r => r.surveyResultId))
            {
                var surveyResult = this.SurveyResultModel.GetOneById(userAnswer.Key).Value;
                if (surveyResult == null)
                {
                    continue;
                }

                var lmsUserParameters = surveyResult.LmsUserParametersId.HasValue ? this.LmsUserParametersModel.GetOneById(surveyResult.LmsUserParametersId.Value).Value : null;
                if (lmsUserParameters == null)
                {
                    return;
                }

                var lmsSurveyId = surveyResult.Survey.LmsSurveyId;
                if (lmsSurveyId == null)
                {
                    continue;
                }

                var converter = this.ConverterFactory.GetResultConverter((LmsProviderEnum)lmsUserParameters.CompanyLms.LmsProviderId);

                converter.ConvertAndSendSurveyResultToLms(results, surveyResult, lmsUserParameters);

                /*
                switch (lmsUserParameters.CompanyLms.LmsProvider.Id)
                {
                    case (int)LmsProviderEnum.Moodle:
                        this.ConvertAndSendSurveyResultToMoodle(userAnswer, lmsUserParameters, surveyResult);
                        break;
                    case (int)LmsProviderEnum.Canvas:
                        this.ConvertAndSendSurveyResultToCanvas(userAnswer, lmsUserParameters, lmsSurveyId.Value);
                        break;
                    case (int)LmsProviderEnum.Blackboard:
                        this.ConvertAndSendSurveyResultToBlackboard(userAnswer, lmsUserParameters, lmsSurveyId.Value, surveyResult.Survey.SubModuleItem.Id);
                        break;
                }
                 * */
            }
        }

        #endregion

    }

}