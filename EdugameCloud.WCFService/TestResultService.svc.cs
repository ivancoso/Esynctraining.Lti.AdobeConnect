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
                    if (this.IsValid(appletResultDTO, out var validationResult))
                    {
                        var sessionModel = this.TestResultModel;
                        var appletResult = this.ConvertDto(appletResultDTO);
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

        private TestResult ConvertDto(TestResultDTO resultDTO)
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
                Test = this.TestModel.GetOneById(resultDTO.testId).Value,
                ACSessionId = this.ACSessionModel.GetOneById(resultDTO.acSessionId).Value.With(x => x.Id),
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

        private TestQuestionResultSaveAllDTO SaveAll(TestResult testResult, TestQuestionResultDTO[] results)
        {
            results = results ?? new TestQuestionResultDTO[] { };

            var result = new TestQuestionResultSaveAllDTO();
            var faults = new List<string>();
            var created = new List<TestQuestionResult>();
            foreach (var appletResultDTO in results)
            {
                if (this.IsValid(appletResultDTO, out var validationResult))
                {
                    var sessionModel = this.TestQuestionResultModel;
                    var appletResult = this.ConvertDto(appletResultDTO, testResult);
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