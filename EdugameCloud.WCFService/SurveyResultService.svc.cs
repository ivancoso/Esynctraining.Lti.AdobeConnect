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
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SurveyResultService : BaseService, ISurveyResultService
    {
        #region Properties

        private SurveyResultModel SurveyResultModel => IoC.Resolve<SurveyResultModel>();

        private SurveyModel SurveyModel => IoC.Resolve<SurveyModel>();

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
            return this.SurveyResultModel.GetAll().Select(x => new SurveyResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="surveyResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyResultDTO"/>.
        /// </returns>
        public SurveyResultDTO Save(SurveyResultDTO surveyResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(surveyResultDTO, out validationResult))
            {
                var surveyResultModel = this.SurveyResultModel;
                var isTransient = surveyResultDTO.surveyResultId == 0;
                var surveyResult = isTransient ? null : surveyResultModel.GetOneById(surveyResultDTO.surveyResultId).Value;
                surveyResult = this.ConvertDto(surveyResultDTO, surveyResult);
                surveyResultModel.RegisterSave(surveyResult);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<SurveyResult>(NotificationType.Update, surveyResultDTO.companyId, surveyResult.Id);
                return new SurveyResultDTO(surveyResult);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("SurveyResult.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
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
            var result = new SurveyResultSaveAllDTO();
            var faults = new List<string>();
            var created = new List<SurveyResult>();
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
                    created.Add(surveyResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<SurveyResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                result.saved = created.Select(x => new SurveyResultDTO(x)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

            return result;
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
            IoC.Resolve<RealTimeNotificationModel>()
                .NotifyClientsAboutChangesInTable<SurveyResult>(
                    NotificationType.Delete,
                    surveyResult.With(x => x.Survey)
                        .With(x => x.SubModuleItem)
                        .With(x => x.CreatedBy)
                        .With(x => x.Company.Id),
                    surveyResult.Id);
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

        #endregion

    }

}