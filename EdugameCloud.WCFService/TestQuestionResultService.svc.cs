namespace EdugameCloud.WCFService
{
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
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The test question result service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class TestQuestionResultService : BaseService, ITestQuestionResultService
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
        /// Gets the test question result model.
        /// </summary>
        private TestQuestionResultModel TestQuestionResultModel
        {
            get
            {
                return IoC.Resolve<TestQuestionResultModel>();
            }
        }

        /// <summary>
        /// Gets the question type model.
        /// </summary>
        private QuestionTypeModel QuestionTypeModel
        {
            get
            {
                return IoC.Resolve<QuestionTypeModel>();
            }
        }

        /// <summary>
        /// Gets the question model.
        /// </summary>
        private QuestionModel QuestionModel
        {
            get
            {
                return IoC.Resolve<QuestionModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Gets all quiz questions results.
        /// </summary>
        /// <returns>
        ///     The <see cref="TestQuestionResultDTO" />.
        /// </returns>
        public TestQuestionResultDTO[] GetAll()
        {
            return this.TestQuestionResultModel.GetAll().Select(x => new TestQuestionResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="TestQuestionResultDTO"/>.
        /// </returns>
        public TestQuestionResultDTO Save(TestQuestionResultDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var quizQuestionResultModel = this.TestQuestionResultModel;
                var isTransient = resultDto.testQuestionResultId == 0;
                var quizQuestionResult = isTransient ? null : quizQuestionResultModel.GetOneById(resultDto.testQuestionResultId).Value;
                quizQuestionResult = this.ConvertDto(resultDto, quizQuestionResult);
                quizQuestionResultModel.RegisterSave(quizQuestionResult);
                return new TestQuestionResultDTO(quizQuestionResult);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("TestQuestionResult.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
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
        public TestQuestionResultSaveAllDTO SaveAll(TestQuestionResultDTO[] results)
        {
            results = results ?? new TestQuestionResultDTO[] { };
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

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TestQuestionResultDTO"/>.
        /// </returns>
        public TestQuestionResultDTO GetById(int id)
        {
            TestQuestionResult quizResult;
            if ((quizResult = this.TestQuestionResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("TestQuestionResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new TestQuestionResultDTO(quizResult);
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
            TestQuestionResult quizResult;
            var model = this.TestQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("TestQuestionResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            return id;
        }

        #endregion

        #region Methods

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

        #endregion
    }
}