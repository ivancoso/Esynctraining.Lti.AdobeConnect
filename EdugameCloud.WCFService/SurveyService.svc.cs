// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
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
    public class SurveyService : BaseService, ISurveyService
    {
        #region Properties

        /// <summary>
        /// Gets the Survey model.
        /// </summary>
        private SurveyModel SurveyModel
        {
            get
            {
                return IoC.Resolve<SurveyModel>();
            }
        }

        /// <summary>
        /// Gets the SurveyGroupingType model.
        /// </summary>
        private SurveyGroupingTypeModel SurveyGroupingTypeModel
        {
            get
            {
                return IoC.Resolve<SurveyGroupingTypeModel>();
            }
        }

        /// <summary>
        /// Gets the LMS user parameters model.
        /// </summary>
        private LmsUserParametersModel LmsUserParametersModel
        {
            get
            {
                return IoC.Resolve<LmsUserParametersModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All items test.
        /// </summary>
        /// <returns>
        ///     The <see cref="SurveyDTO" />.
        /// </returns>
        public SurveyDTO[] GetAll()
        {
            return this.SurveyModel.GetAll().Select(x => new SurveyDTO(x)).ToArray();
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
        /// The <see cref="PagedSurveyDTO"/>.
        /// </returns>
        public PagedSurveyDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedSurveyDTO
            {
                objects = this.SurveyModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new SurveyDTO(x)).ToArray(),
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
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        public SurveyDTO Create(SurveySMIWrapperDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                var quizModel = this.SurveyModel;
                var smiResult = this.Convert(dto.SmiDTO, (SubModuleItem)null, true);
                dto.SurveyDTO.subModuleItemId = smiResult.Id;
                return this.ConvertQuizAndGetServiceResponse(dto.SurveyDTO, null, quizModel);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Survey.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="surveyDTO">
        /// The survey.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        public SurveyDTO Save(SurveyDTO surveyDTO)
        {
            ValidationResult validationResult;
            if (this.IsValid(surveyDTO, out validationResult))
            {
                var quizModel = this.SurveyModel;
                var isTransient = surveyDTO.surveyId == 0;
                var survey = isTransient ? null : quizModel.GetOneById(surveyDTO.surveyId).Value;
                return this.ConvertQuizAndGetServiceResponse(surveyDTO, survey, quizModel);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Survey.Create", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }
        
        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        public SurveyDTO GetById(int id)
        {
            Survey appletResult;
            if ((appletResult = this.SurveyModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Survey.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyDTO(appletResult);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        public SurveyDTO GetBySMIId(int id)
        {
            Survey appletResult;
            if ((appletResult = this.SurveyModel.GetOneBySMIId(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Survey.GetBySMIId", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SurveyDTO(appletResult);
        }

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <param name="showLms">
        /// The show LMS
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        public SurveyFromStoredProcedureDTO[] GetSurveysByUserId(int userId, bool? showLms)
        {
            return this.SurveyModel.GetSurveysByUserId(userId, showLms ?? false).ToArray();
        }

        /// <summary>
        /// The get LMS quizzes.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <param name="lmsUserParametersId">
        /// The LMS User Parameters Id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        public SurveyFromStoredProcedureDTO[] GetLmsSurveys(int userId, int lmsUserParametersId)
        {
            var lmsUserParameters = LmsUserParametersModel.GetOneById(lmsUserParametersId).Value;
            return this.SurveyModel.GetLmsSurveys(userId, lmsUserParameters.Course, lmsUserParameters.CompanyLms.Id).ToArray();
        }

        /// <summary>
        /// The get by user id.
        /// </summary>
        /// <param name="userId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        public SurveyFromStoredProcedureDTO[] GetSharedSurveysByUserId(int userId)
        {
            return this.SurveyModel.GetSharedForUserSurveysByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        public SMICategoriesFromStoredProcedureDTO[] GetSurveyCategoriesbyUserId(int userId)
        {
            return this.SurveyModel.GetSurveyCategoriesbyUserId(userId).ToArray();
        }

        /// <summary>
        /// The get survey sub module items by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyFromStoredProcedureDTO"/>.
        /// </returns>
        public SubModuleItemDTO[] GetSurveySMItemsByUserId(int userId)
        {
            return this.SubModuleItemModel.GetSurveySubModuleItemsByUserId(userId).ToArray();
        }

        /// <summary>
        /// The get quiz data by quiz id.
        /// </summary>
        /// <param name="surveyId">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDataDTO"/>.
        /// </returns>
        public SurveyDataDTO GetSurveyDataBySurveyId(int surveyId)
        {
            return this.SurveyModel.GetSurveyDataBySurveyId(surveyId);
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
        /// The <see cref="Survey"/>.
        /// </returns>
        private Survey ConvertDto(SurveyDTO itemDTO, Survey instance)
        {
            instance = instance ?? new Survey();
            instance.SurveyName = itemDTO.surveyName;
            instance.Description = itemDTO.description;
            instance.SubModuleItem = itemDTO.subModuleItemId.HasValue ? this.SubModuleItemModel.GetOneById(itemDTO.subModuleItemId.Value).Value : null;
            instance.SurveyGroupingType = this.SurveyGroupingTypeModel.GetOneById(itemDTO.surveyGroupingTypeId).Value;
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
        /// <param name="surveyDTO">
        /// The survey DTO.
        /// </param>
        /// <param name="survey">
        /// The survey.
        /// </param>
        /// <param name="quizModel">
        /// The quiz model.
        /// </param>
        /// <returns>
        /// The <see cref="SurveyDTO"/>.
        /// </returns>
        private SurveyDTO ConvertQuizAndGetServiceResponse(
            SurveyDTO surveyDTO,
            Survey survey,
            SurveyModel quizModel)
        {
            survey = this.ConvertDto(surveyDTO, survey);
            quizModel.RegisterSave(survey, true);
            int companyId =
                survey.With(x => x.SubModuleItem).With(x => x.SubModuleCategory).With(x => x.User).With(x => x.Company.Id);
            IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<Survey>(NotificationType.Update, companyId, survey.Id);
            return new SurveyDTO(survey);
        }

        #endregion
    }
}