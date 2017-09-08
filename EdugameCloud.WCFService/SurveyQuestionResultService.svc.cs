// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
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
    using Resources;

    /// <summary>
    ///     The SurveyQuestionResult service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SurveyQuestionResultService : BaseService, ISurveyQuestionResultService
    {
        #region Properties

        private SurveyQuestionResultModel SurveyQuestionResultModel => IoC.Resolve<SurveyQuestionResultModel>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Gets all quiz questions results.
        /// </summary>
        /// <returns>
        ///     The <see cref="SurveyQuestionResultDTO" />.
        /// </returns>
        public SurveyQuestionResultDTO[] GetAll()
        {
            return this.SurveyQuestionResultModel.GetAll().Select(x => new SurveyQuestionResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyQuestionResultDTO"/>.
        /// </returns>
        public SurveyQuestionResultDTO GetById(int id)
        {
            SurveyQuestionResult quizResult;
            if ((quizResult = this.SurveyQuestionResultModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SurveyQuestionResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyQuestionResultDTO(quizResult, quizResult.Answers);
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
            SurveyQuestionResult quizResult;
            var model = this.SurveyQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuestionType.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            return id;
        }

        #endregion
    }
}