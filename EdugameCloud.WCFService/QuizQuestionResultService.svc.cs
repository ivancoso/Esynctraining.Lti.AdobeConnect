// ReSharper disable once CheckNamespace


namespace EdugameCloud.WCFService
{
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.Converters;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;
    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizQuestionResultService : BaseService, IQuizQuestionResultService
    {
        #region Properties

        private QuizQuestionResultModel QuizQuestionResultModel => IoC.Resolve<QuizQuestionResultModel>();

        #endregion

        #region Public Methods and Operators

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
            QuizQuestionResult quizResult;
            QuizQuestionResultModel model = this.QuizQuestionResultModel;
            if ((quizResult = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizQuestionResult.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(quizResult, true);
            return id;
        }

        /// <summary>
        /// Gets all quiz questions results.
        /// </summary>
        /// <returns>
        ///     The <see cref="QuizQuestionResultDTO" />.
        /// </returns>
        public QuizQuestionResultDTO[] GetAll()
        {
            return this.QuizQuestionResultModel.GetAll().Select(x => new QuizQuestionResultDTO(x)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizQuestionResultDTO"/>.
        /// </returns>
        public QuizQuestionResultDTO GetById(int id)
        {
            QuizQuestionResult quizResult;
            if ((quizResult = this.QuizQuestionResultModel.GetOneById(id).Value) == null)
            {
                var error = 
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT, 
                        ErrorsTexts.GetResultError_Subject, 
                        ErrorsTexts.GetResultError_NotFound);
                this.LogError("QuizQuestionResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizQuestionResultDTO(quizResult);
        }

        #endregion
    }
}