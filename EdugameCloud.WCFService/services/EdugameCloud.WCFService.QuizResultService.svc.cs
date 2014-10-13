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

    using Weborb.Client;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizResultService : BaseService, IQuizResultService
    {
        #region Properties

        /// <summary>
        /// Gets the quiz result model.
        /// </summary>
        private QuizResultModel QuizResultModel
        {
            get
            {
                return IoC.Resolve<QuizResultModel>();
            }
        }

        /// <summary>
        /// Gets the Quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All list.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<QuizResultDTO> GetAll()
        {
            return new ServiceResponse<QuizResultDTO> { objects = this.QuizResultModel.GetAll().Select(x => new QuizResultDTO(x)).ToList() };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizResultDTO> Save(QuizResultDTO appletResultDTO)
        {
            var result = new ServiceResponse<QuizResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizResultModel = this.QuizResultModel;
                var isTransient = appletResultDTO.quizResultId == 0;
                var quizResult = isTransient ? null : quizResultModel.GetOneById(appletResultDTO.quizResultId).Value;
                quizResult = this.ConvertDto(appletResultDTO, quizResult);
                quizResultModel.RegisterSave(quizResult);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Update, appletResultDTO.companyId, quizResult.Id);
                result.@object = new QuizResultDTO(quizResult);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
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
        public ServiceResponse<QuizResultDTO> SaveAll(List<QuizResultDTO> results)
        {
            var result = new ServiceResponse<QuizResultDTO>();
            var faults = new List<string>();
            var created = new List<QuizResult>();
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
                    created.Add(appletResult);
                }
                else
                {
                    faults.AddRange(this.UpdateResultToString(validationResult));
                }
            }

            if (created.Any())
            {
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new QuizResultDTO(x)).ToList();
            }

            if (faults.Any())
            {
                result.status = created.Any() ? Errors.CODE_RESULTTYPE_PARTIALSUCCESS : Errors.CODE_RESULTTYPE_ERROR;
                result.SetError(new Error(faults.Any(x => x.StartsWith("108")) ? Errors.CODE_ERRORTYPE_INVALID_SESSION : Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.EntityCreationError_Subject, ErrorsTexts.EntityCreation_PartialSuccessMessage, faults));
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
        public ServiceResponse<QuizResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<QuizResultDTO>();
            QuizResult quizResult;
            if ((quizResult = this.QuizResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new QuizResultDTO(quizResult);
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
            QuizResult quizResult;
            var model = this.QuizResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(quizResult, true);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Delete, quizResult.With(x => x.Quiz).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id), quizResult.Id);
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
        private QuizResult ConvertDto(QuizResultDTO resultDTO, QuizResult instance)
        {
            instance = instance ?? new QuizResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime;
            instance.EndTime = resultDTO.endTime;
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.ACEmail = resultDTO.acEmail.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            instance.DateCreated = resultDTO.dateCreated == DateTime.MinValue ? DateTime.Now : resultDTO.dateCreated;
            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.Quiz = this.QuizModel.GetOneById(resultDTO.quizId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.LmsId = resultDTO.lmsId;
            instance.isCompleted = resultDTO.isCompleted;
            return instance;
        }

        #endregion
    }
}