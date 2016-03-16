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
        ///     The <see cref="QuizResultDTO" />.
        /// </returns>
        public QuizResultDTO[] GetAll()
        {
            return this.QuizResultModel.GetAll().Select(x => new QuizResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultDTO"/>.
        /// </returns>
        public QuizResultDTO Save(QuizResultDTO appletResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizResultModel = this.QuizResultModel;
                var isTransient = appletResultDTO.quizResultId == 0;
                var quizResult = isTransient ? null : quizResultModel.GetOneById(appletResultDTO.quizResultId).Value;
                quizResult = this.ConvertDto(appletResultDTO, quizResult);
                quizResultModel.RegisterSave(quizResult);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Update, appletResultDTO.companyId, quizResult.Id);
                return new QuizResultDTO(quizResult);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("QuizResult.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="QuizResultSaveAllDTO"/>.
        /// </returns>
        public QuizResultSaveAllDTO SaveAll(QuizResultDTO[] results)
        {
            results = results ?? new QuizResultDTO[] { };
            var result = new QuizResultSaveAllDTO();
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
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                result.saved = created.Select(x => new QuizResultDTO(x)).ToArray();
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
        /// The <see cref="QuizResultDTO"/>.
        /// </returns>
        public QuizResultDTO GetById(int id)
        {
            QuizResult quizResult;
            if ((quizResult = this.QuizResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizResultDTO(quizResult);
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
            QuizResult quizResult;
            var model = this.QuizResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<QuizResult>(NotificationType.Delete, quizResult.With(x => x.Quiz).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id), quizResult.Id);
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
        private QuizResult ConvertDto(QuizResultDTO resultDTO, QuizResult instance)
        {
            instance = instance ?? new QuizResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp();
            instance.EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp();
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.ACEmail = resultDTO.acEmail.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.Quiz = this.QuizModel.GetOneById(resultDTO.quizId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.LmsId = resultDTO.lmsId;
            instance.LmsUserParametersId = resultDTO.lmsUserParametersId > 0 ? new int?(resultDTO.lmsUserParametersId) : null;
            instance.isCompleted = resultDTO.isCompleted;
            return instance;
        }

        #endregion
    }
}