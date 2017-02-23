namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
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

        /// <summary>
        ///     All items test.
        /// </summary>
        /// <returns>
        ///     The <see cref="QuizDTO" />.
        /// </returns>
        public QuizDTO[] GetAll()
        {
            return this.QuizModel.GetAll().Select(x => new QuizDTO(x)).ToArray();
        }

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="PagedQuizDTO"/>.
        /// </returns>
        public PagedQuizDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedQuizDTO
            {
                objects = this.QuizModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new QuizDTO(x)).ToArray(),
                totalCount = totalCount
            };
        }

        /// <summary>
        /// The creation of quiz.
        /// </summary>
        /// <param name="dto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
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

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
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

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
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

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <param name="showLms">
        /// The show LMS.
        /// </param>
        /// <returns>
        /// The <see cref="QuizFromStoredProcedureDTO"/>.
        /// </returns>
        public QuizFromStoredProcedureDTO[] GetQuizzesByUserId(int userId, bool? showLms)
        {
            return
                this.QuizModel.GetQuizzesByUserId(userId, showLms ?? false)
                    .ToList()
                    .Select(x => new QuizFromStoredProcedureDTO(x))
                    .ToArray();
        }

        /// <summary>
        /// The get LMS quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizFromStoredProcedureDTO"/>.
        /// </returns>
        public QuizFromStoredProcedureDTO[] GetLmsQuizzes(int userId, int lmsUserParametersId)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;
            return
                this.QuizModel.GetLMSQuizzes(userId, lmsUserParameters.Course, lmsUserParameters.CompanyLms.Id)
                    .ToList()
                    .Select(x => new QuizFromStoredProcedureDTO(x))
                    .ToArray();
        }

        /// <summary>
        /// The get by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizFromStoredProcedureDTO"/>.
        /// </returns>
        public QuizFromStoredProcedureDTO[] GetSharedQuizzesByUserId(int userId)
        {
            return
                this.QuizModel.GetSharedForUserQuizzesByUserId(userId)
                    .ToList()
                    .Select(x => new QuizFromStoredProcedureDTO(x))
                    .ToArray();
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SMICategoriesFromStoredProcedureDTO"/>.
        /// </returns>
        public SMICategoriesFromStoredProcedureDTO[] GetQuizCategoriesbyUserId(int userId)
        {
            return this.QuizModel.GetQuizCategoriesbyUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItemDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetQuizSMItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz data by quiz id.
        /// </summary>
        /// <param name="quizId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDataDTO"/>.
        /// </returns>
        public QuizDataDTO GetQuizDataByQuizId(int quizId)
        {
            return this.QuizModel.getQuizDataByQuizID(quizId);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="itemDTO">
        /// The user.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="Quiz"/>.
        /// </returns>
        private Quiz ConvertDto(QuizDTO itemDTO, Quiz instance)
        {
            instance = instance ?? new Quiz();
            instance.QuizName = itemDTO.quizName.With(x => x.Trim());
            instance.Description = itemDTO.description.With(x => x.Trim());
            instance.IsPostQuiz = itemDTO.isPostQuiz;
            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
            instance.QuizFormat = itemDTO.quizFormatId.HasValue ? this.QuizFormatModel.GetOneById(itemDTO.quizFormatId.Value).Value ?? this.QuizFormatModel.GetOneById(1).Value : this.QuizFormatModel.GetOneById(1).Value;
            instance.ScoreType = itemDTO.scoreTypeId.HasValue ? this.ScoreTypeModel.GetOneById(itemDTO.scoreTypeId.Value).Value ?? this.ScoreTypeModel.GetOneById(1).Value : this.ScoreTypeModel.GetOneById(1).Value;
            if (instance.SubModuleItem != null)
            {
                instance.SubModuleItem.DateModified = DateTime.Now;
                this.SubModuleItemModel.RegisterSave(instance.SubModuleItem);
            }

            return instance;
        }

        /// <summary>
        /// The convert quiz and get service response.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The applet result DTO.
        /// </param>
        /// <param name="quiz">
        /// The quiz.
        /// </param>
        /// <param name="quizModel">
        /// The quiz model.
        /// </param>
        /// <returns>
        /// The <see cref="QuizDTO"/>.
        /// </returns>
        private QuizDTO ConvertQuizAndGetServiceResponse(
            QuizDTO appletResultDTO,
            Quiz quiz,
            QuizModel quizModel)
        {
            quiz = this.ConvertDto(appletResultDTO, quiz);
            quizModel.RegisterSave(quiz, true);
            int companyId =
                quiz.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Quiz>(NotificationType.Update, companyId, quiz.Id);
            return new QuizDTO(quiz);
        }

        #endregion
    }
}