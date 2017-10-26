using System.Threading.Tasks;

namespace EdugameCloud.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using Newtonsoft.Json;

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

        public async Task SaveAll(TestSummaryResultDTO testResult)
        {
            if (testResult == null)
                throw new ArgumentNullException(nameof(testResult));
            if (testResult.testResults == null)
                testResult.testResults = new TestResultDTO[0];

            try
            {
                if (!IsValid(testResult, out var valResult))
                    return;

                Test test = TestModel.GetOneById(testResult.testId).Value;
                ACSession acSession = ACSessionModel.GetOneById(testResult.acSessionId).Value;
                if (acSession == null)
                {
                    throw new ArgumentException($"There are not session with acSessionId : {testResult.acSessionId}");
                }
                
                int acSessionId = acSession.With(x => x.Id);

                foreach (var appletResultDTO in testResult.testResults)
                {
                    appletResultDTO.acSessionId = testResult.acSessionId;
                    if (IsValid(appletResultDTO, out var validationResult))
                    {
                        var appletResult = ConvertDto(appletResultDTO, test, acSessionId);
                        TestResultModel.RegisterSave(appletResult);
                        if (appletResultDTO.results != null)
                        {
                            SaveAll(appletResult, appletResultDTO.results);
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Logger.Error($"TestResultService.SaveAll: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"TestResultService.SaveAll json={JsonConvert.SerializeObject(testResult)}", ex);

                throw;
            }
        }

        #endregion

        #region Methods

        private static TestResult ConvertDto(TestResultDTO resultDTO, Test test, int acSessionId)
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
                Test = test,
                ACSessionId = acSessionId,
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
            foreach (var appletResultDTO in results)
            {
                if (IsValid(appletResultDTO, out var validationResult))
                {
                    var appletResult = ConvertDto(appletResultDTO, testResult);
                    TestQuestionResultModel.RegisterSave(appletResult);
                }
            }
        }

        #endregion

    }

}