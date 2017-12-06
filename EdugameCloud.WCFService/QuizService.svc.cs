namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
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
            if (IsValid(dto, out ValidationResult validationResult))
            {
                var smiResult = Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.QuizDTO.subModuleItemId = smiResult.Id;
                return ConvertQuizAndGetServiceResponse(dto.QuizDTO, null, QuizModel);
            }

            var error = GenerateValidationError(validationResult);
            LogError("Quiz.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public QuizDTO Save(QuizDTO appletResultDTO)
        {
            if (this.IsValid(appletResultDTO, out ValidationResult validationResult))
            {
                var quizModel = QuizModel;
                var isTransient = appletResultDTO.quizId == 0;
                var quiz = isTransient ? null : quizModel.GetOneById(appletResultDTO.quizId).Value;
                return ConvertQuizAndGetServiceResponse(appletResultDTO, quiz, quizModel);
            }

            var error = GenerateValidationError(validationResult);
            LogError("Quiz.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        public QuizDTO GetById(int id)
        {
            Quiz appletResult;
            if ((appletResult = QuizModel.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                LogError("Quiz.GetById", error);
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
            if ((appletResult = QuizModel.GetOneBySMIId(id).Value) == null)
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
                QuizModel.GetQuizzesByUserId(userId, showLms ?? false)
                .Select(x => new QuizFromStoredProcedureDTO(x))
                .ToArray();
        }

        public QuizFromStoredProcedureDTO[] GetLmsQuizzes(int userId, int lmsUserParametersId)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;
            return
                QuizModel.GetLMSQuizzes(userId, lmsUserParameters.Course, lmsUserParameters.CompanyLms.Id)
                .Select(x => new QuizFromStoredProcedureDTO(x))
                .ToArray();
        }

        public QuizFromStoredProcedureDTO[] GetSharedQuizzesByUserId(int userId)
        {
            return
                QuizModel.GetSharedForUserQuizzesByUserId(userId)
                .Select(x => new QuizFromStoredProcedureDTO(x))
                .ToArray();
        }

        public SMICategoriesFromStoredProcedureDTO[] GetQuizCategoriesbyUserId(int userId)
        {
            return QuizModel.GetQuizCategoriesbyUserId(userId).ToArray();
        }

        public SubModuleItemDTO[] GetQuizSMItemsByUserId(int userId)
        {
            return SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToArray();
        }

        public QuizDataDTO GetQuizDataByQuizId(int quizId)
        {
            return QuizModel.getQuizDataByQuizID(quizId);
        }

        #endregion

        #region Methods

        private Quiz ConvertDto(QuizDTO itemDTO, Quiz instance)
        {
            instance = instance ?? new Quiz();
            instance.QuizName = itemDTO.quizName?.Trim();
            instance.Description = itemDTO.description?.Trim();
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
            quiz = ConvertDto(appletResultDTO, quiz);
            quizModel.RegisterSave(quiz, true);
            return new QuizDTO(quiz);
        }

        #endregion

    }

}