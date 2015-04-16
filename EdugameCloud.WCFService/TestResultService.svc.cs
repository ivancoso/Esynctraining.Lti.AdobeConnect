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
    using EdugameCloud.Core.Extensions;
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
        ///     The <see cref="TestResultDTO" />.
        /// </returns>
        public TestResultDTO[] GetAll()
        {
            return this.TestResultModel.GetAll().Select(x => new TestResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="TestResultDTO"/>.
        /// </returns>
        public TestResultDTO Save(TestResultDTO appletResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizResultModel = this.TestResultModel;
                var isTransient = appletResultDTO.testResultId == 0;
                var quizResult = isTransient ? null : quizResultModel.GetOneById(appletResultDTO.testResultId).Value;
                quizResult = this.ConvertDto(appletResultDTO, quizResult);
                quizResultModel.RegisterSave(quizResult);
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<TestResult>(NotificationType.Update, appletResultDTO.companyId, quizResult.Id);
                return new TestResultDTO(quizResult);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("TestResult.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="TestResultSaveAllDTO"/>.
        /// </returns>
        public TestResultSaveAllDTO SaveAll(TestResultDTO[] results)
        {
            results = results ?? new TestResultDTO[] { };
            var result = new TestResultSaveAllDTO();
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
                IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<TestResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                result.saved = created.Select(x => new TestResultDTO(x)).ToArray();
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
        /// The <see cref="TestResultDTO"/>.
        /// </returns>
        public TestResultDTO GetById(int id)
        {
            TestResult quizResult;
            if ((quizResult = this.TestResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("TestResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new TestResultDTO(quizResult);
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
            TestResult quizResult;
            var model = this.TestResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("TestResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            IoC.Resolve<RealTimeNotificationModel>()
                .NotifyClientsAboutChangesInTable<TestResult>(
                    NotificationType.Delete,
                    quizResult.With(x => x.Test)
                        .With(x => x.SubModuleItem)
                        .With(x => x.CreatedBy)
                        .With(x => x.Company.Id),
                    quizResult.Id);
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
        /// The <see cref="TestResult"/>.
        /// </returns>
        private TestResult ConvertDto(TestResultDTO resultDTO, TestResult instance)
        {
            instance = instance ?? new TestResult();
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
            instance.Test = this.TestModel.GetOneById(resultDTO.testId).Value;
            instance.ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id);
            instance.IsCompleted = resultDTO.isCompleted;
            return instance;
        }

        #endregion
    }
}