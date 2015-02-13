// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SurveyResultService : BaseService, ISurveyResultService
    {
        #region Properties

        /// <summary>
        /// Gets the survey result model.
        /// </summary>
        private SurveyResultModel SurveyResultModel
        {
            get
            {
                return IoC.Resolve<SurveyResultModel>();
            }
        }

        /// <summary>
        /// Gets the Survey model.
        /// </summary>
        private SurveyModel SurveyModel
        {
            get
            {
                return IoC.Resolve<SurveyModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All results.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<SurveyResultDTO> GetAll()
        {
            return new ServiceResponse<SurveyResultDTO> { objects = this.SurveyResultModel.GetAll().Select(x => new SurveyResultDTO(x)).ToList() };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="surveyResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyResultDTO> Save(SurveyResultDTO surveyResultDTO)
        {
            var result = new ServiceResponse<SurveyResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(surveyResultDTO, out validationResult))
            {
                var surveyResultModel = this.SurveyResultModel;
                var isTransient = surveyResultDTO.surveyResultId == 0;
                var surveyResult = isTransient ? null : surveyResultModel.GetOneById(surveyResultDTO.surveyResultId).Value;
                surveyResult = this.ConvertDto(surveyResultDTO, surveyResult);
                surveyResultModel.RegisterSave(surveyResult);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<SurveyResult>(NotificationType.Update, surveyResultDTO.companyId, surveyResult.Id);
                result.@object = new SurveyResultDTO(surveyResult);
                return result;
            }

            result = (ServiceResponse<SurveyResultDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyResultDTO> SaveAll(List<SurveyResultDTO> results)
        {
            var result = new ServiceResponse<SurveyResultDTO>();
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
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<SurveyResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new SurveyResultDTO(x)).ToList();
            }

            if (faults.Any())
            {
                result.SetError(new Error(faults.Any(x => x.StartsWith("108")) ? Errors.CODE_ERRORTYPE_INVALID_SESSION : Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
            }

            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SurveyResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<SurveyResultDTO>();
            SurveyResult surveyResult;
            if ((surveyResult = this.SurveyResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SurveyResultDTO(surveyResult);
            }

            return result;
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            SurveyResult surveyResult;
            var model = this.SurveyResultModel;
            if ((surveyResult = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(surveyResult, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<SurveyResult>(NotificationType.Delete, surveyResult.With(x => x.Survey).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id), surveyResult.Id);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
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
            instance.StartTime = resultDTO.startTime;
            instance.EndTime = resultDTO.endTime;
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            instance.DateCreated = resultDTO.dateCreated == DateTime.MinValue ? DateTime.Now : resultDTO.dateCreated;
            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.Survey = this.SurveyModel.GetOneById(resultDTO.surveyId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.LmsUserParametersId = resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null;
            return instance;
        }

        #endregion
    }
}