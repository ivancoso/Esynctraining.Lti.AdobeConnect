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
    public class TestResultService : BaseService, ITestResultService
    {
        #region Properties

        /// <summary>
        /// Gets the test result model.
        /// </summary>
        private TestResultModel TestResultModel
        {
            get
            {
                return IoC.Resolve<TestResultModel>();
            }
        }

        /// <summary>
        /// Gets the Test model.
        /// </summary>
        private TestModel TestModel
        {
            get
            {
                return IoC.Resolve<TestModel>();
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
        public ServiceResponse<TestResultDTO> GetAll()
        {
            return new ServiceResponse<TestResultDTO> { objects = this.TestResultModel.GetAll().Select(x => new TestResultDTO(x)).ToList() };
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
        public ServiceResponse<TestResultDTO> Save(TestResultDTO appletResultDTO)
        {
            var result = new ServiceResponse<TestResultDTO>();
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizResultModel = this.TestResultModel;
                var isTransient = appletResultDTO.testResultId == 0;
                var quizResult = isTransient ? null : quizResultModel.GetOneById(appletResultDTO.testResultId).Value;
                quizResult = this.ConvertDto(appletResultDTO, quizResult);
                quizResultModel.RegisterSave(quizResult);
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<TestResult>(NotificationType.Update, appletResultDTO.companyId, quizResult.Id);
                result.@object = new TestResultDTO(quizResult);
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
        public ServiceResponse<TestResultDTO> SaveAll(List<TestResultDTO> results)
        {
            var result = new ServiceResponse<TestResultDTO>();
            var faults = new List<string>();
            var created = new List<TestResult>();
            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.TestResultModel;
                    var isTransient = appletResultDTO.testResultId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.testResultId).Value;
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
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<TestResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.objects = created.Select(x => new TestResultDTO(x)).ToList();
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
        public ServiceResponse<TestResultDTO> GetById(int id)
        {
            var result = new ServiceResponse<TestResultDTO>();
            TestResult quizResult;
            if ((quizResult = this.TestResultModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new TestResultDTO(quizResult);
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
            TestResult quizResult;
            var model = this.TestResultModel;
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
                IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<TestResult>(NotificationType.Delete, quizResult.With(x => x.Test).With(x => x.SubModuleItem).With(x => x.CreatedBy).With(x => x.Company.Id), quizResult.Id);
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
        /// The <see cref="TestResult"/>.
        /// </returns>
        private TestResult ConvertDto(TestResultDTO resultDTO, TestResult instance)
        {
            instance = instance ?? new TestResult();
            instance.Score = resultDTO.score;
            instance.StartTime = resultDTO.startTime;
            instance.EndTime = resultDTO.endTime;
            instance.Email = resultDTO.email.With(x => x.Trim());
            instance.ACEmail = resultDTO.acEmail.With(x => x.Trim());
            instance.IsArchive = resultDTO.isArchive;
            instance.DateCreated = resultDTO.dateCreated == DateTime.MinValue ? DateTime.Now : resultDTO.dateCreated;
            instance.ParticipantName = resultDTO.participantName.With(x => x.Trim());
            instance.Test = this.TestModel.GetOneById(resultDTO.testId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.IsCompleted = resultDTO.isCompleted;
            return instance;
        }

        #endregion
    }
}