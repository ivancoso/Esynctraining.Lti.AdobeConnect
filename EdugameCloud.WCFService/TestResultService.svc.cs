﻿// ReSharper disable once CheckNamespace
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
    //using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

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
    public class TestResultService : BaseService, ITestResultService
    {
        #region Properties

        private TestResultModel TestResultModel => IoC.Resolve<TestResultModel>();

        private TestQuestionResultModel TestQuestionResultModel => IoC.Resolve<TestQuestionResultModel>();

        private QuestionTypeModel QuestionTypeModel => IoC.Resolve<QuestionTypeModel>();

        private QuestionModel QuestionModel => IoC.Resolve<QuestionModel>();

        private TestModel TestModel => IoC.Resolve<TestModel>();

        #endregion

        #region Public Methods and Operators

        public TestResultSaveAllDTO SaveAll(TestResultDTO[] results)
        {
            results = results ?? new TestResultDTO[] { };

            try
            {
                var result = new TestResultSaveAllDTO();
                var faults = new List<string>();
                var created = new List<TestResultSaveResultDTO>();
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

                        var testSaveResult = new TestResultSaveResultDTO(appletResult);
                        created.Add(testSaveResult);

                        var testQuestionResult = SaveAll(appletResult, appletResultDTO.results);
                        testSaveResult.testQuestionResult = testQuestionResult;
                    }
                    else
                    {
                        faults.AddRange(this.UpdateResultToString(validationResult));
                    }
                }

                if (created.Any())
                {
                    //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<TestResult>(NotificationType.Update, results.FirstOrDefault().With(x => x.companyId), 0);
                    result.saved = created.ToArray();
                }

                if (faults.Any())
                {
                    result.faults = faults.ToArray();
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"TestResultService.SaveAll json={JsonConvert.SerializeObject(results)}", ex);

                throw;
            }
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
        private TestQuestionResult ConvertDto(TestQuestionResultDTO resultDTO, TestQuestionResult instance)
        {
            instance = instance ?? new TestQuestionResult();
            instance.Question = resultDTO.question;
            instance.IsCorrect = resultDTO.isCorrect;
            instance.QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value;
            instance.TestResult = this.TestResultModel.GetOneById(resultDTO.testResultId).Value;
            instance.QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value;
            return instance;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="results">
        /// The applet Result DTOs.
        /// </param>
        /// <returns>
        /// The <see cref="TestQuestionResultDTO"/>.
        /// </returns>
        private TestQuestionResultSaveAllDTO SaveAll(TestResult testResult, TestQuestionResultDTO[] results)
        {
            results = results ?? new TestQuestionResultDTO[] { };

            foreach (var item in results)
            {
                item.testResultId = testResult.Id;
            }

            var result = new TestQuestionResultSaveAllDTO();
            var faults = new List<string>();
            var created = new List<TestQuestionResult>();
            foreach (var appletResultDTO in results)
            {
                ValidationResult validationResult;
                if (this.IsValid(appletResultDTO, out validationResult))
                {
                    var sessionModel = this.TestQuestionResultModel;
                    var isTransient = appletResultDTO.testQuestionResultId == 0;
                    var appletResult = isTransient ? null : sessionModel.GetOneById(appletResultDTO.testQuestionResultId).Value;
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
                result.saved = created.Select(x => new TestQuestionResultDTO(x)).ToArray();
            }

            if (faults.Any())
            {
                result.faults = faults.ToArray();
            }

            return result;
        }

        #endregion
    }
}