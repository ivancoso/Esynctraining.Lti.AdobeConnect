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

        private TestResultModel _testResultModel;
        private TestResultModel TestResultModel
        {
            get
            {
                return _testResultModel ?? (_testResultModel = IoC.Resolve<TestResultModel>());
            }
        }

        private TestQuestionResultModel _testQuestionResultModel;
        private TestQuestionResultModel TestQuestionResultModel
        {
            get
            {
                return _testQuestionResultModel ?? (_testQuestionResultModel = IoC.Resolve<TestQuestionResultModel>());
            }
        }

        private QuestionTypeModel _questionTypeModel;
        private QuestionTypeModel QuestionTypeModel
        {
            get { return _questionTypeModel ?? (_questionTypeModel = IoC.Resolve<QuestionTypeModel>()); }
        }

        private QuestionModel _questionModel;
        private QuestionModel QuestionModel
        {
            get { return _questionModel ?? (_questionModel = IoC.Resolve<QuestionModel>()); }
        }

        private TestModel _testModel;
        private TestModel TestModel
        {
            get
            {
                return _testModel ?? (_testModel = IoC.Resolve<TestModel>());
            }
        }

        #endregion

        #region Public Methods and Operators

        public TestResultSaveAllDTO SaveAll(TestSummaryResultDTO testResult)
        {
            testResult = testResult ?? new TestSummaryResultDTO { testResults = new TestResultDTO[] {}};

            try
            {
                var result = new TestResultSaveAllDTO();
                foreach (var appletResultDTO in testResult.testResults)
                {
                    if (this.IsValid(appletResultDTO, out var validationResult))
                    {
                        var sessionModel = this.TestResultModel;
                        var appletResult = this.ConvertDto(appletResultDTO, testResult);
                        sessionModel.RegisterSave(appletResult);
                        SaveAll(appletResult, appletResultDTO.results);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"TestResultService.SaveAll json={JsonConvert.SerializeObject(testResult)}", ex);

                throw;
            }
        }

        #endregion

        #region Methods

        private TestResult ConvertDto(TestResultDTO resultDTO, TestSummaryResultDTO testResult)
        {
            var instance = new TestResult
            {
                Score = resultDTO.score,
                StartTime = resultDTO.startTime.ConvertFromUnixTimeStamp(),
                EndTime = resultDTO.endTime.ConvertFromUnixTimeStamp(),
                Email = resultDTO.email.With(x => x.Trim()),
                ACEmail = resultDTO.acEmail.With(x => x.Trim()),
                IsArchive = resultDTO.isArchive,
                DateCreated = DateTime.Now,
                ParticipantName = resultDTO.participantName.With(x => x.Trim()),
                Test = this.TestModel.GetOneById(testResult.testId).Value,
                ACSessionId = this.ACSessionModel.GetOneById(testResult.acSessionId).Value.With(x => x.Id),
                IsCompleted = resultDTO.isCompleted
            };

            return instance;
        }

        private TestQuestionResult ConvertDto(TestQuestionResultDTO resultDTO, TestResult testResult)
        {
            var instance = new TestQuestionResult
            {
                Question = resultDTO.question,
                IsCorrect = resultDTO.isCorrect,
                QuestionType = this.QuestionTypeModel.GetOneById(resultDTO.questionTypeId).Value,
                TestResult = testResult,
                QuestionRef = this.QuestionModel.GetOneById(resultDTO.questionId).Value
            };
            return instance;
        }

        private void SaveAll(TestResult testResult, TestQuestionResultDTO[] results)
        {
            results = results ?? new TestQuestionResultDTO[] { };

            foreach (var appletResultDTO in results)
            {
                if (this.IsValid(appletResultDTO, out var validationResult))
                {
                    var sessionModel = this.TestQuestionResultModel;
                    var appletResult = this.ConvertDto(appletResultDTO, testResult);
                    sessionModel.RegisterSave(appletResult);
                }
            }
        }

        #endregion
    }
}