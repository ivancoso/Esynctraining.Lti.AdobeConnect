namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    //using EdugameCloud.Core.RTMP;
    using EdugameCloud.Lti.Core.Business.Models;
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
    public class QuizService : BaseService, IQuizService
    {
        #region Properties

        private QuizModel QuizModel => IoC.Resolve<QuizModel>();

        private LmsUserParametersModel LmsUserParametersModel => IoC.Resolve<LmsUserParametersModel>();

        private QuizFormatModel QuizFormatModel => IoC.Resolve<QuizFormatModel>();

        private ScoreTypeModel ScoreTypeModel => IoC.Resolve<ScoreTypeModel>();

        #endregion

        #region Public Methods and Operators
        
        public QuizDTO Create(QuizSMIWrapperDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var quizModel = this.QuizModel;
                var smiResult = this.Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.QuizDTO.subModuleItemId = smiResult.Id;
                return this.ConvertQuizAndGetServiceResponse(dto.QuizDTO, null, quizModel);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Quiz.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public QuizDTO Save(QuizDTO appletResultDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizModel = this.QuizModel;
                var isTransient = appletResultDTO.quizId == 0;
                var quiz = isTransient ? null : quizModel.GetOneById(appletResultDTO.quizId).Value;
                return this.ConvertQuizAndGetServiceResponse(appletResultDTO, quiz, quizModel);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Quiz.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public QuizDTO GetById(int id)
        {
            Quiz appletResult;
            if ((appletResult = this.QuizModel.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("Quiz.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizDTO(appletResult);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
        public QuizDTO GetBySMIId(int id)
        {
            Quiz appletResult;
            if ((appletResult = this.QuizModel.GetOneBySMIId(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("Quiz.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new QuizDTO(appletResult);
        }

        public QuizFromStoredProcedureDTO[] GetQuizzesByUserId(int userId, bool? showLms)
        {
            return
                this.QuizModel.GetQuizzesByUserId(userId, showLms ?? false)
                    .Select(x => new QuizFromStoredProcedureDTO(x))
                    .ToArray();
        }

        public QuizFromStoredProcedureDTO[] GetLmsQuizzes(int userId, int lmsUserParametersId)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;
            return
                this.QuizModel.GetLMSQuizzes(userId, lmsUserParameters.Course, lmsUserParameters.CompanyLms.Id)
                    .Select(x => new QuizFromStoredProcedureDTO(x))
                    .ToArray();
        }

        public QuizFromStoredProcedureDTO[] GetSharedQuizzesByUserId(int userId)
        {
            return
                this.QuizModel.GetSharedForUserQuizzesByUserId(userId)
                    .Select(x => new QuizFromStoredProcedureDTO(x))
                    .ToArray();
        }

        public SMICategoriesFromStoredProcedureDTO[] GetQuizCategoriesbyUserId(int userId)
        {
            return this.QuizModel.GetQuizCategoriesbyUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetQuizSMItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToArray();
        }

        public QuizDataDTO GetQuizDataByQuizId(int quizId)
        {
            return this.QuizModel.getQuizDataByQuizID(quizId);
        }

        #endregion

        #region Methods

        private Quiz ConvertDto(QuizDTO itemDTO, Quiz instance)
        {
            instance = instance ?? new Quiz();
            instance.QuizName = itemDTO.quizName.With(x => x.Trim());
            instance.Description = itemDTO.description.With(x => x.Trim());
            instance.IsPostQuiz = itemDTO.isPostQuiz;
            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
            instance.QuizFormat = itemDTO.quizFormatId.HasValue ? this.QuizFormatModel.GetOneById(itemDTO.quizFormatId.Value).Value ?? this.QuizFormatModel.GetOneById(1).Value : this.QuizFormatModel.GetOneById(1).Value;
            //hardcoded. 3=percentage, this value is not used at the moment but probably will be used in future
            instance.ScoreType = ScoreTypeModel.GetOneById(3).Value; //itemDTO.scoreTypeId.HasValue ? this.ScoreTypeModel.GetOneById(itemDTO.scoreTypeId.Value).Value ?? this.ScoreTypeModel.GetOneById(1).Value : this.ScoreTypeModel.GetOneById(1).Value;
            instance.PassingScore = itemDTO.PassingScore;
            if (instance.SubModuleItem != null)
            {
                instance.SubModuleItem.DateModified = DateTime.Now;
                this.SubModuleItemModel.RegisterSave(instance.SubModuleItem);
            }

            return instance;
        }

        private QuizDTO ConvertQuizAndGetServiceResponse(
            QuizDTO appletResultDTO,
            Quiz quiz,
            QuizModel quizModel)
        {
            quiz = this.ConvertDto(appletResultDTO, quiz);
            quizModel.RegisterSave(quiz, true);
            int companyId =
                quiz.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            //IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Quiz>(NotificationType.Update, companyId, quiz.Id);
            return new QuizDTO(quiz);
        }

        #endregion

    }

}