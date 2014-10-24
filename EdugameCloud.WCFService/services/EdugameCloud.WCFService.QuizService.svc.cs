// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.RTMP;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuizService : BaseService, IQuizService
    {
        #region Properties

        /// <summary>
        /// Gets the Quiz model.
        /// </summary>
        private QuizModel QuizModel
        {
            get
            {
                return IoC.Resolve<QuizModel>();
            }
        }

        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        /// <summary>
        /// Gets the QuizFormat model.
        /// </summary>
        private QuizFormatModel QuizFormatModel
        {
            get
            {
                return IoC.Resolve<QuizFormatModel>();
            }
        }

        /// <summary>
        /// Gets the Score model.
        /// </summary>
        private ScoreTypeModel ScoreTypeModel
        {
            get
            {
                return IoC.Resolve<ScoreTypeModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All items test.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<QuizDTO> GetAll()
        {
            return new ServiceResponse<QuizDTO> { objects = this.QuizModel.GetAll().Select(x => new QuizDTO(x)).ToList() };
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<QuizDTO>
            {
                objects = this.QuizModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new QuizDTO(x)).ToList(),
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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizDTO> Create(QuizSMIWrapperDTO dto)
        {
            var result = new ServiceResponse<QuizDTO>();
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var quizModel = this.QuizModel;
                var smiResult = this.Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.QuizDTO.subModuleItemId = smiResult.Id;
                return this.ConvertQuizAndGetServiceResponse(dto.QuizDTO, null, quizModel, result);
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="appletResultDTO">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizDTO> Save(QuizDTO appletResultDTO)
        {
            var result = new ServiceResponse<QuizDTO>();
            ValidationResult validationResult;
            if (this.IsValid(appletResultDTO, out validationResult))
            {
                var quizModel = this.QuizModel;
                var isTransient = appletResultDTO.quizId == 0;
                var quiz = isTransient ? null : quizModel.GetOneById(appletResultDTO.quizId).Value;
                return this.ConvertQuizAndGetServiceResponse(appletResultDTO, quiz, quizModel, result);
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizDTO> GetById(int id)
        {
            var result = new ServiceResponse<QuizDTO>();
            Quiz appletResult;
            if ((appletResult = this.QuizModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new QuizDTO(appletResult);
            }

            return result;
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizDTO> GetBySMIId(int id)
        {
            var result = new ServiceResponse<QuizDTO>();
            Quiz appletResult;
            if ((appletResult = this.QuizModel.GetOneBySMIId(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new QuizDTO(appletResult);
            }

            return result;
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <param name="showLms">
        /// The show lms.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizFromStoredProcedureDTO> GetQuizzesByUserId(int userId, bool? showLms)
        {
            return new ServiceResponse<QuizFromStoredProcedureDTO> { objects = this.QuizModel.GetQuizzesByUserId(userId, showLms ?? false).ToList() };
        }

        /// <summary>
        /// The get lms quizzes.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The lms user parameters id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizFromStoredProcedureDTO> GetLmsQuizzes(int userId, int lmsUserParametersId)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;

            return new ServiceResponse<QuizFromStoredProcedureDTO> { objects = this.QuizModel.GetLMSQuizzes(userId, lmsUserParameters.Course, lmsUserParameters.CompanyLms.LmsProvider.Id).ToList() };
        }

        /// <summary>
        /// The get by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizFromStoredProcedureDTO> GetSharedQuizzesByUserId(int userId)
        {
            return new ServiceResponse<QuizFromStoredProcedureDTO> { objects = this.QuizModel.GetSharedForUserQuizzesByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SMICategoriesFromStoredProcedureDTO> GetQuizCategoriesbyUserId(int userId)
        {
            return new ServiceResponse<SMICategoriesFromStoredProcedureDTO> { objects = this.QuizModel.GetQuizCategoriesbyUserId(userId).ToList() };
        }

        /// <summary>
        /// The get quiz sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubModuleItemDTO> GetQuizSMItemsByUserId(int userId)
        {
            return new ServiceResponse<SubModuleItemDTO> { objects = this.SubModuleItemModel.GetQuizSMItemsByUserId(userId).ToList() };
        }

        /// <summary>
        /// The get quiz data by quiz id.
        /// </summary>
        /// <param name="quizId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<QuizDataDTO> GetQuizDataByQuizId(int quizId)
        {
            return new ServiceResponse<QuizDataDTO> { @object = this.QuizModel.getQuizDataByQuizID(quizId) };
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
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private ServiceResponse<QuizDTO> ConvertQuizAndGetServiceResponse(
            QuizDTO appletResultDTO,
            Quiz quiz,
            QuizModel quizModel,
            ServiceResponse<QuizDTO> result)
        {
            quiz = this.ConvertDto(appletResultDTO, quiz);
            quizModel.RegisterSave(quiz, true);
            int companyId =
                quiz.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<Quiz>(NotificationType.Update, companyId, quiz.Id);
            result.@object = new QuizDTO(quiz);
            return result;
        }

        #endregion
    }
}