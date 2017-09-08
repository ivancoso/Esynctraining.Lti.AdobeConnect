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

        private TestQuestionResultModel TestQuestionResultModel => IoC.Resolve<TestQuestionResultModel>();

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
    }
}